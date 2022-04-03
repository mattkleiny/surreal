using Surreal.Automata.BehaviourTrees;

namespace Isaac.Core.Behaviours;

[Behaviour(Name = "Remove Coins", Description = "Removes coins from the owning object's inventory.")]
public sealed record RemoveCoins(int Coins) : BehaviourTask
{
  protected override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    if (context.Properties.Get(Properties.Coins) > 0)
    {
      context.Properties.Decrement(Properties.Coins, Coins);

      return BehaviourStatus.Success;
    }

    return BehaviourStatus.Failure;
  }
}
