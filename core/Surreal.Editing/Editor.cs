using Surreal.Commands;
using Surreal.Utilities;

namespace Surreal;

/// <summary>Static facade for editor functionality.</summary>
public static class Editor
{
  /// <summary>The top-level editor services.</summary>
  public static IServiceRegistry Services { get; internal set; } = null!;

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
}
