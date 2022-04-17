using System.Windows.Input;

namespace Surreal.Commands;

/// <summary>Static factory for <see cref="EditorCommand{T}"/>s.</summary>
public static class EditorCommand
{
  public static EditorCommand<T> Create<T>(T instance) => new(() => instance);
  public static EditorCommand<T> Create<T>(Func<T> factory) => new(factory);
}

/// <summary>A <see cref="ICommand"/> that gets dispatched to the <see cref="Editor"/>.</summary>
public sealed class EditorCommand<TCommand> : ICommand
{
  private readonly Func<TCommand> commandFactory;

  public EditorCommand(Func<TCommand> commandFactory)
  {
    this.commandFactory = commandFactory;
  }

  public event EventHandler? CanExecuteChanged;

  public bool CanExecute(object? parameter)
  {
    return true;
  }

  public void Execute(object? parameter)
  {
    Editor.ExecuteCommandAsync(commandFactory());
  }
}
