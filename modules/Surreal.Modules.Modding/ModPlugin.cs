using System.ComponentModel.Design;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;
using Surreal.Collections;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;

namespace Surreal;

/// <summary>A <see cref="GamePlugin{TGame}"/> for mod support.</summary>
public sealed class ModPlugin : GamePlugin
{
  private static readonly ILog      Log      = LogFactory.GetLog<ModPlugin>();
  private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<ModPlugin>();

  private readonly CompositionHost host;
  private          InstalledMod[]? mods;

  public ModPlugin(Game game)
    : this(game, basePath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods"))
  {
  }

  public ModPlugin(Game game, string basePath, string searchPattern = "*Mod*.dll")
    : base(game)
  {
    Registry = new ModRegistry(Game.Services);

    host = BuildCompositionHost(basePath, searchPattern);
  }

  public IModRegistry                Registry  { get; }
  public ReadOnlySlice<InstalledMod> Installed => mods ?? ReadOnlySlice<InstalledMod>.Empty;

  public override Task InitializeAsync()
  {
    mods = LoadMods();

    foreach (var mod in Installed)
    {
      Log.Trace($"Initializing mod: {mod.Metadata.Name} {mod.Metadata.Version}");

      mod.Instance.Initialize(Registry);
    }

    return base.InitializeAsync();
  }

  public override void Input(GameTime time)
  {
    if (mods != null)
    {
      using var _ = Profiler.Track(nameof(Input));

      foreach (var mod in mods)
      {
        mod.Instance.Input(time.DeltaTime);
      }
    }
  }

  public override void Update(GameTime time)
  {
    if (mods != null)
    {
      using var _ = Profiler.Track(nameof(Update));

      foreach (var mod in mods)
      {
        mod.Instance.Update(time.DeltaTime);
      }
    }
  }

  public override void Draw(GameTime time)
  {
    if (mods != null)
    {
      using var _ = Profiler.Track(nameof(Draw));

      foreach (var mod in mods)
      {
        mod.Instance.Draw(time.DeltaTime);
      }
    }
  }

  public override void Dispose()
  {
    if (mods != null)
    {
      foreach (var mod in mods)
      {
        mod.Instance.Dispose();
      }
    }

    host.Dispose();

    base.Dispose();
  }

  private InstalledMod[] LoadMods()
  {
    InstalledMod[] LoadModsInner()
    {
      return host
        .GetExports<ExportFactory<IMod, ExportModAttribute>>()
        .Select(factory => new InstalledMod(factory))
        .ToArray();
    }

    return Log.Profile("Loading mods", LoadModsInner);
  }

  private static CompositionHost BuildCompositionHost(string basePath, string searchPattern)
  {
    if (!Directory.Exists(basePath))
    {
      Directory.CreateDirectory(basePath);
    }

    var configuration = new ContainerConfiguration()
      .WithAssembly(Assembly.GetEntryAssembly())
      .WithAssemblies(Directory
        .GetFiles(basePath, searchPattern, SearchOption.AllDirectories)
        .Select(Assembly.LoadFile));

    return configuration.CreateContainer();
  }

  /// <summary>The default <see cref="IModRegistry"/> implementation.</summary>
  private sealed class ModRegistry : IModRegistry
  {
    public ModRegistry(IServiceContainer services)
    {
      Services = services;
    }

    public IServiceContainer Services { get; }
  }

  /// <summary>Represents a mod that is installed in the game.</summary>
  [DebuggerDisplay("{Metadata.Name} v{Metadata.Version}")]
  public sealed class InstalledMod
  {
    public InstalledMod(ExportFactory<IMod, ExportModAttribute> factory)
    {
      Instance = factory.CreateExport().Value;
      Metadata = ExtractMetadata(factory.Metadata);
    }

    public IMod         Instance { get; }
    public IModMetadata Metadata { get; }

    private ExportModAttribute ExtractMetadata(ExportModAttribute attribute)
    {
      if (string.IsNullOrEmpty(attribute.Name)) attribute.Name       = Instance.GetType().Name;
      if (string.IsNullOrEmpty(attribute.Version)) attribute.Version = "0.1";

      return attribute;
    }
  }
}
