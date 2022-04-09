namespace Surreal.Commands;

/// <summary>Moves a unit to some position in world space.</summary>
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

/// <summary>The <see cref="CommandDescriptor"/>  for <see cref="MoveToPositionCommand"/>.</summary>
public sealed record MoveToPositionDescriptor(Vector2 Position) : CommandDescriptor("Move To")
{
  public override ICommand CreateCommand()
  {
    return new MoveToPositionCommand(Position);
  }
}
