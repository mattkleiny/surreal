using Surreal.Conditions;
using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Decorators;

/// <summary>A <see cref="BehaviourDecorator"/> that executes the child node if the given <see cref="ICondition"/> evaluates to true.</summary>
public sealed record CheckCondition<T>(BehaviourNode Child, ICondition Condition) : BehaviourDecorator(Child)
  where T : IEquatable<T>
{
  protected internal override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    var conditionContext = new ConditionContext(context.Owner, context.Properties);

    if (Condition.Evaluate(conditionContext))
    {
      return Child.Update(context, deltaTime);
    }

    return BehaviourStatus.Failure;
  }
}
