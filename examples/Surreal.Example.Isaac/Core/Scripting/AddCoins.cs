using Surreal.Scripting;
using Surreal.Utilities;

namespace Isaac.Core.Scripting;

[EditorDescription(
  Name = "Add coins",
  Category = "Inventory",
  Description = "Adds coins to the owning object's inventory"
)]
public sealed record AddCoins(int Coins) : IAction
{
  public ValueTask ExecuteAsync(ActionContext context)
  {
    if (context.Properties.Get(PropertyTypes.Coins) < 99)
    {
      context.Properties.Increment(PropertyTypes.Coins, Coins);
    }

    return ValueTask.CompletedTask;
  }
}
