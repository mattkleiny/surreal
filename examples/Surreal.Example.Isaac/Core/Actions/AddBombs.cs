using Surreal.Actions;
using Surreal.Utilities;

namespace Isaac.Core.Actions;

[Export(Name = "Add Bombs", Description = "Adds bombs to the owning object's inventory.")]
public sealed record AddBombs(int Bombs) : IAction
{
  public ValueTask ExecuteAsync(ActionContext context)
  {
    if (context.Properties.Get(Properties.Bombs) < 99)
    {
      context.Properties.Increment(Properties.Bombs, Bombs);
    }

    return ValueTask.CompletedTask;
  }
}
