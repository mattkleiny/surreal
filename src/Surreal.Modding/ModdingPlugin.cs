using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Composition;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Surreal.Diagnostics.Logging;
using Surreal.Framework;

namespace Surreal {
  public sealed class ModdingPlugin : GamePlugin {
    private static readonly ILog Log = LogFactory.GetLog<ModdingPlugin>();

    private readonly CompositionHost                  host;
    private readonly Lazy<IEnumerable<IInstalledMod>> mods;

    public ModdingPlugin(Game game)
        : this(game, basePath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods")) {
    }

    public ModdingPlugin(Game game, string basePath, string searchPattern = "*Mod*.dll")
        : base(game) {
      Check.NotNullOrEmpty(basePath, nameof(basePath));

      Registry = new ModRegistry(Game.Services);

      host = BuildCompositionHost(basePath, searchPattern);
      mods = new Lazy<IEnumerable<IInstalledMod>>(LoadMods);
    }

    public IEnumerable<IInstalledMod> Installed => mods.Value;

    public IModRegistry Registry { get; }

    public override void Initialize() {
      base.Initialize();

      foreach (var mod in Installed) {
        Log.Trace($"Initializing mod: {mod.Metadata.Name} {mod.Metadata.Version}");

        mod.Instance.Initialize(Registry);
      }
    }

    public override void Begin() {
      base.Begin();

      foreach (var mod in Installed) {
        mod.Instance.Begin();
      }
    }

    public override void Input(GameTime time) {
      base.Input(time);

      foreach (var mod in Installed) {
        mod.Instance.Input(time.DeltaTime);
      }
    }

    public override void Update(GameTime time) {
      base.Update(time);

      foreach (var mod in Installed) {
        mod.Instance.Update(time.DeltaTime);
      }
    }

    public override void Draw(GameTime time) {
      base.Draw(time);

      foreach (var mod in Installed) {
        mod.Instance.Draw(time.DeltaTime);
      }
    }

    public override void End() {
      base.End();

      foreach (var mod in Installed) {
        mod.Instance.End();
      }
    }

    public override void Dispose() {
      foreach (var mod in Installed) {
        mod.Instance.Dispose();
      }

      host?.Dispose();

      base.Dispose();
    }

    private IEnumerable<IInstalledMod> LoadMods() {
      return Log.Profile("Loading mods", () => host
          .GetExports<ExportFactory<IMod, ExportModAttribute>>()
          .Select(factory => new InstalledMod(factory))
          .ToArray());
    }

    private static CompositionHost BuildCompositionHost(string basePath, string searchPattern) {
      if (!Directory.Exists(basePath)) {
        Directory.CreateDirectory(basePath);
      }

      var configuration = new ContainerConfiguration()
          .WithAssembly(Assembly.GetEntryAssembly())
          .WithAssemblies(Directory
              .GetFiles(basePath, searchPattern, SearchOption.AllDirectories)
              .Select(Assembly.LoadFile));

      return configuration.CreateContainer();
    }

    private sealed class ModRegistry : IModRegistry {
      public ModRegistry(IServiceContainer services) {
        Services = services;
      }

      public IServiceContainer Services { get; }
    }

    [DebuggerDisplay("{Metadata.Name} v{Metadata.Version}")]
    private sealed class InstalledMod : IInstalledMod {
      public InstalledMod(ExportFactory<IMod, ExportModAttribute> factory) {
        Instance = factory.CreateExport().Value;
        Metadata = ExtractMetadata(factory.Metadata);
      }

      public IMod         Instance { get; }
      public IModMetadata Metadata { get; }

      private ExportModAttribute ExtractMetadata(ExportModAttribute attribute) {
        if (string.IsNullOrEmpty(attribute.Name)) attribute.Name       = Instance.GetType().Name;
        if (string.IsNullOrEmpty(attribute.Version)) attribute.Version = "0.1";

        return attribute;
      }
    }
  }
}