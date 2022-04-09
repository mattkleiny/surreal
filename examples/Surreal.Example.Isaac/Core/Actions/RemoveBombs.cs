using Surreal.Actions;
using Surreal.Utilities;

namespace Isaac.Core.Actions;

[Export(Name = "Remove Bombs", Description = "Removes bombs from the owning object's inventory.")]
public sealed record RemoveBombs(int Bombs) : IAction
{
  public ValueTask ExecuteAsync(ActionContext context)
  {
    if (context.Properties.Get(Properties.Bombs) > 0)
    {
      context.Properties.Decrement(Properties.Bombs, Bombs);
    }

    return ValueTask.CompletedTask;
  }
}
