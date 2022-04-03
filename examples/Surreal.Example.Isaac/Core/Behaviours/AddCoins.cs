using Surreal.Automata.BehaviourTrees;

namespace Isaac.Core.Behaviours;

[Behaviour(Name = "Add Coins", Description = "Adds coins to the owning object's inventory.")]
public sealed record AddCoins(int Coins) : BehaviourTask
{
  protected override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    if (context.Properties.Get(Properties.Coins) < 99)
    {
      context.Properties.Increment(Properties.Coins, Coins);

      return BehaviourStatus.Success;
    }

    return BehaviourStatus.Failure;
  }
}
