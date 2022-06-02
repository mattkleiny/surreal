using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Tasks;

/// <summary>A <see cref="BehaviourTask"/> that delays a fixed amount of time.</summary>
public sealed record FixedDelay(TimeSpan Duration) : BehaviourTask
{
  private IntervalTimer timer;

  protected internal override void OnEnter(in BehaviourContext context)
  {
    base.OnEnter(context);

    timer = new IntervalTimer(Duration);
  }

  protected internal override BehaviourStatus OnUpdate(in BehaviourContext context, TimeDelta deltaTime)
  {
    if (timer.Tick(deltaTime))
    {
      return BehaviourStatus.Success;
    }

    return BehaviourStatus.Running;
  }
}
