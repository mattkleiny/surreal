using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Decorators;

/// <summary>A <see cref="BehaviourDecorator"/> that always returns success on child nodes.</summary>
public sealed record AlwaysSuccess(BehaviourNode Child) : BehaviourDecorator(Child)
{
  protected internal override BehaviourStatus OnUpdate(BehaviourContext context, DeltaTime deltaTime)
  {
    var status = Child.Update(context, deltaTime);

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
