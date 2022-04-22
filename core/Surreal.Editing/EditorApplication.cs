using Avalonia;
using Surreal.Commands;

namespace Surreal;

/// <summary>An Avalonia <see cref="EditorApplication"/> for a <see cref="TGame"/> editor.</summary>
public abstract class EditorApplication<TGame> : EditorApplication
  where TGame : Game, new()
{
  /// <summary>The top-level <see cref="TGame"/> instance.</summary>
  public TGame Game { get; } = new();
}

/// <summary>Base class for any editor <see cref="Application"/>.</summary>
public abstract class EditorApplication : Application
{
  public new static EditorApplication? Current => (EditorApplication?) Application.Current;

  /// <summary>The top-level <see cref="IServiceProvider"/>.</summary>
  public IServiceProvider Services { get; private set; } = null!;

  /// <summary>The <see cref="EditorWorkload"/>s to enable for the game.</summary>
  public abstract IEnumerable<EditorWorkload> Workloads { get; }

  public override void Initialize()
  {
    Services = CreateServiceRegistry();

    base.Initialize();
  }

  /// <summary>Builds the top-level <see cref="IServiceRegistry"/> for the editor.</summary>
  private IServiceRegistry CreateServiceRegistry()
  {
    IServiceRegistry registry = new ServiceRegistry();

    registry.AddSingleton<IEditorBus, EditorBus>();

    foreach (var workload in Workloads)
    {
      registry.AddModule(workload.Services);
    }

    return registry;
  }
}
