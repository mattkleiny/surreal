using Avalonia;
using Surreal.Commands;
using Surreal.Internal;

namespace Surreal;

/// <summary>An Avalonia <see cref="Application"/> for a <see cref="TGame"/> editor.</summary>
public abstract class EditorApplication<TGame> : Application
  where TGame : Game, new()
{
  /// <summary>The top-level <see cref="TGame"/> instance.</summary>
  public TGame Game { get; } = new();

  /// <summary>The <see cref="EditorWorkload"/>s to enable for the game.</summary>
  public abstract IEnumerable<EditorWorkload> Workloads { get; }

  public override void Initialize()
  {
    Editor.Services = CreateServiceRegistry();

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
