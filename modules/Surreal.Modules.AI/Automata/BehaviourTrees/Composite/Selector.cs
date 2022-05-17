﻿using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Composite;

/// <summary>A <see cref="BehaviourComposite"/> that executes the first successful task from among children.</summary>
public sealed record Selector : BehaviourComposite
{
  private int lastIndex;

  public Selector(params BehaviourNode[] children)
  {
    Children = ImmutableList.CreateRange(children);
  }

  protected internal override void OnExit(in BehaviourContext context)
  {
    base.OnExit(context);

    lastIndex = 0;
  }

  protected internal override BehaviourStatus OnUpdate(in BehaviourContext context, TimeDelta deltaTime)
  {
    for (; lastIndex < Children.Count; lastIndex++)
    {
      var child = Children[lastIndex];

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
