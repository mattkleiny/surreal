using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Decorators;

/// <summary>A <see cref="BehaviourDecorator"/> that always returns success on child nodes.</summary>
public sealed record AlwaysSuccess(BehaviourNode Child) : BehaviourDecorator(Child)
{
  protected internal override BehaviourStatus OnUpdate(BehaviourContext context, DeltaTime deltaTime)
  {
    return Child.Update(context, deltaTime) switch
    {
      BehaviourStatus.Sleeping => BehaviourStatus.Sleeping,
      BehaviourStatus.Running  => BehaviourStatus.Running,
      BehaviourStatus.Success  => BehaviourStatus.Success,
      BehaviourStatus.Failure  => BehaviourStatus.Success,

      _ => throw new ArgumentOutOfRangeException()
    };
  }
}
