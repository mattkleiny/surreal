using Surreal.Collections;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal.AI.BehaviourTrees.Decorators;

/// <summary>A <see cref="BehaviourDecorator"/> that executes the child node if the given property is the given value..</summary>
public sealed record CompareProperty<T>(BehaviourNode Child, Property<T> Property, ValueComparison<T> Comparison) : BehaviourDecorator(Child)
  where T : IComparable<T>
{
  protected internal override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    if (Comparison.Compare(context.Properties.Get(Property)))
    {
      return Child.Update(context, deltaTime);
    }

    return BehaviourStatus.Failure;
  }
}
