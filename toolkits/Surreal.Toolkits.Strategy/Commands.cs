namespace Surreal;

public readonly struct CommandContext
{
  public object Sender   { get; init; }
  public object UserData { get; init; }
}

public interface ICommand
{
  bool CanExecute(CommandContext context);
  void Execute(CommandContext context);
}

public record MoveToPositionCommand(Vector2 Position) : ICommand
{
  public bool CanExecute(CommandContext context)
  {
    throw new NotImplementedException();
  }

  public void Execute(CommandContext context)
  {
    throw new NotImplementedException();
  }
}

public sealed class AnonymousCommand : ICommand
{
  private readonly Func<CommandContext, bool> canExecute;
  private readonly Action<CommandContext>     execute;

  public AnonymousCommand(Action<CommandContext> execute)
    : this(_ => true, execute)
  {
  }

  public AnonymousCommand(Func<CommandContext, bool> canExecute, Action<CommandContext> execute)
  {
    this.canExecute = canExecute;
    this.execute    = execute;
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

public interface ICommandProvider
{
  IEnumerable<ICommand> GetCommands(CommandContext context);
}

public sealed class SelectedUnitCommandProvider : ICommandProvider
{
  private readonly IUnitSelectionProvider selectionProvider;

  public SelectedUnitCommandProvider(IUnitSelectionProvider selectionProvider)
  {
    this.selectionProvider = selectionProvider;
  }

  public IEnumerable<ICommand> GetCommands(CommandContext context)
  {
    return selectionProvider.SelectedUnits.OfType<ICommandProvider>().SelectMany(_ => _.GetCommands(context));
  }
}
