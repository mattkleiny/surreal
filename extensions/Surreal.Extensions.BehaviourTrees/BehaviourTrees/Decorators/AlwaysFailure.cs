using Surreal.Timing;

namespace Surreal.BehaviourTrees.Decorators;

/// <summary>
/// A <see cref="BehaviourDecorator"/> that always returns failure on child nodes.
/// </summary>
public sealed record AlwaysFailure(BehaviourNode Child) : BehaviourDecorator(Child)
{
  protected override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    var status = Child.Update(context, deltaTime);

    return status switch
    {
      BehaviourStatus.Sleeping => BehaviourStatus.Sleeping,
      BehaviourStatus.Running  => BehaviourStatus.Running,
      BehaviourStatus.Success  => BehaviourStatus.Failure,
      BehaviourStatus.Failure  => BehaviourStatus.Failure,

      _ => throw new InvalidOperationException($"An unrecognized status was encountered: {status}"),
    };
  }
}
