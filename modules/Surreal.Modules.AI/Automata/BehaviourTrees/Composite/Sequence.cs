using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Composite;

/// <summary>A <see cref="BehaviourComposite"/> that executes nodes in order.</summary>
public sealed record Sequence : BehaviourComposite
{
  private int lastIndex;

  public Sequence(params BehaviourNode[] children)
  {
    Children = ImmutableList.CreateRange(children);
  }

  protected internal override void OnExit(BehaviourContext context)
  {
    base.OnExit(context);

    lastIndex = 0;
  }

  protected internal override BehaviourStatus OnUpdate(BehaviourContext context, DeltaTime deltaTime)
  {
    for (; lastIndex < Children.Count; lastIndex++)
    {
      var child = Children[lastIndex];

      switch (child.Update(context, deltaTime))
      {
        case BehaviourStatus.Running:
          return BehaviourStatus.Running;

        case BehaviourStatus.Failure:
          return BehaviourStatus.Failure;
      }
    }

    return BehaviourStatus.Success;
  }
}
