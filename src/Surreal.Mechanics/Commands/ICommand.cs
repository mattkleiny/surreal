using Surreal.Fibers;

namespace Surreal.Mechanics.Commands
{
  public interface ICommand
  {
    FiberTask<CommandResult> ExecuteAsync(object target);
  }
}
