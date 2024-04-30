using System.Windows.Input;

namespace Surreal.Editing.Common;

/// <summary>
/// A <see cref="ICommand"/> that executes a delegate, with an optional predicate.
/// </summary>
public sealed class EditorCommand(Action callback, Func<bool> predicate) : ICommand
{
  public EditorCommand(Action callback)
    : this(callback, static () => true)
  {
  }

  /// <inheritdoc/>
  public event EventHandler? CanExecuteChanged;

  /// <inheritdoc/>
  public bool CanExecute(object? parameter)
  {
    return predicate();
  }

  /// <inheritdoc/>
  public void Execute(object? parameter)
  {
    callback();
  }

  /// <summary>
  /// Notifies that the command's execution state has changed.
  /// </summary>
  public void NotifyCanExecuteChanged()
  {
    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
  }
}
