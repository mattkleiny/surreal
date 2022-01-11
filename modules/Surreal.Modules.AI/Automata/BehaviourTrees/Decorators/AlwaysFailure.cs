using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Decorators;

/// <summary>A <see cref="BehaviourDecorator"/> that always returns failure on child nodes.</summary>
public sealed record AlwaysFailure(BehaviourNode Child) : BehaviourDecorator(Child)
{
  protected internal override BehaviourStatus OnUpdate(BehaviourContext context, DeltaTime deltaTime)
  {
    return Child.Update(context, deltaTime) switch
    {
      BehaviourStatus.Sleeping => BehaviourStatus.Sleeping,
      BehaviourStatus.Running  => BehaviourStatus.Running,
      BehaviourStatus.Success  => BehaviourStatus.Failure,
      BehaviourStatus.Failure  => BehaviourStatus.Failure,

      _ => throw new ArgumentOutOfRangeException()
    };
  }
}
