using Surreal.Actions;
using Surreal.Utilities;

namespace Isaac.Core.Actions;

[EditorDescription(
  Name = "Remove coins",
  Category = "Inventory",
  Description = "Removes coins from the owning object's inventory"
)]
public sealed record RemoveCoins(int Coins) : IAction
{
  public ValueTask ExecuteAsync(ActionContext context)
  {
    if (context.Properties.Get(PropertyTypes.Coins) > 0)
    {
      context.Properties.Decrement(PropertyTypes.Coins, Coins);
    }

    return ValueTask.CompletedTask;
  }
}
