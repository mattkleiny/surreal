using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Decorators;

/// <summary>A <see cref="BehaviourDecorator"/> that executes the child node if the given property is the given value.</summary>
public sealed record CheckProperty<T>(BehaviourNode Child, Property<T> Property, T Value) : BehaviourDecorator(Child)
  where T : IEquatable<T>
{
  protected internal override BehaviourStatus OnUpdate(in BehaviourContext context, TimeDelta deltaTime)
  {
    if (Value.Equals(context.Properties.Get(Property)))
    {
      return Child.Update(context, deltaTime);
    }

    return BehaviourStatus.Failure;
  }
}
