namespace Surreal;

public readonly struct CommandContext
{
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

public interface ICommandProvider
{
  event Action CommandsChanged;

  IEnumerable<ICommand> GetCommands(CommandContext context);
}

public sealed class SelectedUnitCommandProvider : ICommandProvider
{
  private readonly IUnitSelectionManager selectionManager;

  public SelectedUnitCommandProvider(IUnitSelectionManager selectionManager)
  {
    this.selectionManager = selectionManager;

    selectionManager.SelectionChanged += OnSelectionChanged;
  }

  public event Action? CommandsChanged;

  public IEnumerable<ICommand> GetCommands(CommandContext context)
  {
    return selectionManager.SelectedUnits.OfType<ICommandProvider>().SelectMany(_ => _.GetCommands(context));
  }

  private void OnSelectionChanged()
  {
    CommandsChanged?.Invoke();
  }
}

public sealed class CommandPalette
{
  public CommandPalette(ICommandProvider commandProvider)
  {
  }

  public void Refresh()
  {
  }
}
