using Surreal.Automata.BehaviourTrees;

namespace Isaac.Core.Behaviours;

public sealed record AddCoins(int Coins) : BehaviourTask
{
  protected override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    context.Properties.Increment(Properties.Coins, Coins);

    return BehaviourStatus.Success;
  }
}
