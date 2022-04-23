using Surreal.Mathematics;
using Surreal.Timing;

namespace Surreal.AI.BehaviourTrees.Tasks;

/// <summary>A <see cref="BehaviourTask"/> that delays a fixed amount of time.</summary>
public sealed record RandomDelay(TimeSpanRange DurationRange) : BehaviourTask
{
  private IntervalTimer timer;

  protected internal override void OnEnter(in BehaviourContext context)
  {
    base.OnEnter(context);

    timer = new IntervalTimer(Random.Shared.NextRange(DurationRange));
  }

  protected internal override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    if (timer.Tick(deltaTime))
    {
      return BehaviourStatus.Success;
    }

    return BehaviourStatus.Running;
  }
}
