using Surreal.Timing;

namespace Surreal.Behaviours.Decorators;

/// <summary>
/// A <see cref="BehaviourDecorator"/> that always returns success on child nodes.
/// </summary>
public sealed class AlwaysSuccess(BehaviourNode child) : BehaviourDecorator
{
  protected override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    var status = child.Update(context, deltaTime);

    return status switch
    {
      BehaviourStatus.Sleeping => BehaviourStatus.Sleeping,
      BehaviourStatus.Running  => BehaviourStatus.Running,
      BehaviourStatus.Success  => BehaviourStatus.Success,
      BehaviourStatus.Failure  => BehaviourStatus.Success,

      _ => throw new InvalidOperationException($"An unrecognized status was encountered: {status}"),
    };
  }
}
