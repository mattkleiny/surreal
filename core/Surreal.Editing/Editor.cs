using System.Reflection;
using Surreal.Commands;
using Surreal.Internal;
using Surreal.Utilities;

namespace Surreal;

/// <summary>Static facade for editor functionality.</summary>
public static class Editor
{
  /// <summary>The editor services.</summary>
  public static IServiceRegistry Services { get; } = CreateServiceRegistry();

  /// <summary>Top-level view model for the entire editor.</summary>
  public static EditorViewModel ViewModel { get; } = GetViewModel<EditorViewModel>();

  /// <summary>Resolves the given view model, or creates a new one if it doesn't exist.</summary>
  public static TViewModel GetViewModel<TViewModel>()
    where TViewModel : ViewModel, new()
  {
    return new TViewModel();
  }

  /// <summary>Executes the given editor command.</summary>
  public static ValueTask ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
  {
    var bus = Services.GetRequiredService<IEditorBus>();

    return bus.ExecuteCommandAsync(command, cancellationToken);
  }

  /// <summary>Builds the top-level <see cref="IServiceRegistry"/> for the editor.</summary>
  private static IServiceRegistry CreateServiceRegistry()
  {
    IServiceRegistry registry = new ServiceRegistry();

    registry.AddAssemblyServices(Assembly.GetExecutingAssembly());

    var entryAssembly = Assembly.GetEntryAssembly();
    if (entryAssembly != null)
    {
      registry.AddAssemblyServices(entryAssembly);
    }

    return registry;
  }
}
