using Surreal.Actions;
using Surreal.Utilities;

namespace Isaac.Core.Actions;

[Export(Name = "Add Coins", Description = "Adds coins to the owning object's inventory.")]
public sealed record AddCoins(int Coins) : IAction
{
  public ValueTask ExecuteAsync(ActionContext context)
  {
    if (context.Properties.Get(Properties.Coins) < 99)
    {
      context.Properties.Increment(Properties.Coins, Coins);
    }

    return ValueTask.CompletedTask;
  }
}
