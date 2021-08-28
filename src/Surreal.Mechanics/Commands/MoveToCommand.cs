using Surreal.Fibers;
using Surreal.Mathematics.Linear;
using Surreal.Mechanics.Abstractions;

namespace Surreal.Mechanics.Commands
{
  public record MoveToCommand(Point2 Position) : Command
  {
    protected override async FiberTask<CommandResult> ExecuteAsync(object target)
    {
      if (target is not IMovable movable)
      {
        return CommandResult.Failure("The target is not movable!");
      }

      await movable.MoveTo(Position);

      return CommandResult.Success();
    }
  }
}
