using Surreal.Actions;
using Surreal.Utilities;

namespace Isaac.Core.Actions;

[EditorDescription(
  Name = "Remove bombs",
  Category = "Inventory",
  Description = "Removes bombs from the owning object's inventory"
)]
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
