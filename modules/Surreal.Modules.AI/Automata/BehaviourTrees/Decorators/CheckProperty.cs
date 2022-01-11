using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Decorators;

/// <summary>A <see cref="BehaviourDecorator"/> that executes the child node if the given property is the given value..</summary>
public sealed record CheckProperty<T>(BehaviourNode Child, Property<T> Property, T Value) : BehaviourDecorator(Child)
{
  protected internal override BehaviourStatus OnUpdate(BehaviourContext context, DeltaTime deltaTime)
  {
    if (Equals(context.Properties.Get(Property), Value))
    {
      return Child.Update(context, deltaTime);
    }

    return BehaviourStatus.Failure;
  }
}
