using Surreal.Fibers;

namespace Surreal.Mechanics.Commands
{
  public interface ICommand
  {
    FiberTask<CommandResult> ExecuteAsync(object target);
  }

  public abstract record Command : ICommand
  {
    protected virtual FiberTask<CommandResult> ExecuteAsync(object target)
    {
      return FiberTask.FromResult(Execute(target));
    }

    protected virtual CommandResult Execute(object target)
    {
      return CommandResult.Failure("This command is not yet implemented!");
    }

    FiberTask<CommandResult> ICommand.ExecuteAsync(object target)
    {
      return ExecuteAsync(target);
    }
  }
}
