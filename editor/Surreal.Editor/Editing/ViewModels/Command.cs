using System.Windows.Input;

namespace Surreal.Editing.ViewModels;

/// <summary>
/// A <see cref="ICommand"/> that executes a delegate, with an optional predicate.
/// </summary>
public sealed class Command : ICommand
{
  private readonly Action _callback;
  private readonly Func<bool> _predicate;

  public Command(Action callback)
    : this(callback, static () => true)
  {
  }

  public Command(Action callback, Func<bool> predicate)
  {
    _callback = callback;
    _predicate = predicate;
  }

  /// <inheritdoc/>
  public event EventHandler? CanExecuteChanged;

  /// <inheritdoc/>
  public bool CanExecute(object? parameter)
  {
    return _predicate();
  }

  /// <inheritdoc/>
  public void Execute(object? parameter)
  {
    _callback();
  }

  /// <summary>
  /// Notifies that the command's execution state has changed.
  /// </summary>
  public void NotifyCanExecuteChanged()
  {
    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
  }
}
