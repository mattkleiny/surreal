using System;
using System.Windows.Input;

namespace Surreal.Editor.Internal
{
  internal sealed class Command : ICommand
  {
    private readonly Action     action;
    private readonly Func<bool> canExecute;

    public Command(Action action, Func<bool>? canExecute = default)
    {
      this.action     = action;
      this.canExecute = canExecute ?? (() => true);
    }

    public event EventHandler? CanExecuteChanged
    {
      add => CommandManager.RequerySuggested += value;
      remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => canExecute.Invoke();
    public void Execute(object? parameter)    => action();
  }
}