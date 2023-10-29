using Surreal.Timing;

namespace Surreal.BehaviourTrees.Composite;

/// <summary>
/// A <see cref="BehaviourComposite"/> that executes the first successful task from among children.
/// </summary>
public sealed record Selector : BehaviourComposite
{
  private int _lastIndex;

  protected override void OnExit(in BehaviourContext context)
  {
    base.OnExit(context);

    _lastIndex = 0;
  }

  protected override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    for (; _lastIndex < Children.Count; _lastIndex++)
    {
      var child = Children[_lastIndex];

      switch (child.Update(context, deltaTime))
      {
        case BehaviourStatus.Running:
          return BehaviourStatus.Running;

        case BehaviourStatus.Success:
          return BehaviourStatus.Success;

        case BehaviourStatus.Failure:
          continue;
      }
    }

    return BehaviourStatus.Failure;
  }
}
