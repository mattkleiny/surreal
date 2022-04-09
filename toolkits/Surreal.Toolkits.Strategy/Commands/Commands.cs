namespace Surreal.Commands;

/// <summary>A command that can be executed against some object.</summary>
public interface ICommand
{
  bool CanExecute(CommandContext context);
  void Execute(CommandContext context);
}

/// <summary>Context for <see cref="ICommand"/> operations.</summary>
public readonly struct CommandContext
{
  public Vector2 Position { get; init; }
  public object? Sender   { get; init; }
  public object? UserData { get; init; }
}

/// <summary>An anonymous, delegate-based <see cref="ICommand"/> implementation.</summary>
public sealed class AnonymousCommand : ICommand
{
  private readonly Func<CommandContext, bool> canExecute;
  private readonly Action<CommandContext> execute;

  public AnonymousCommand(Action<CommandContext> execute)
    : this(_ => true, execute)
  {
  }

  public AnonymousCommand(Func<CommandContext, bool> canExecute, Action<CommandContext> execute)
  {
    this.canExecute = canExecute;
    this.execute = execute;
  }

  public bool CanExecute(CommandContext context)
  {
    return canExecute(context);
  }

  public void Execute(CommandContext context)
  {
    execute(context);
  }
}
