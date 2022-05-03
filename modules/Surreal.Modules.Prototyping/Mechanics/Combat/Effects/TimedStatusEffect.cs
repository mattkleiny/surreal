using Surreal.Timing;

namespace Surreal.Mechanics.Combat.Effects;

/// <summary>A <see cref="StatusEffect"/> that ticks over a duration.</summary>
public abstract class TimedStatusEffect : StatusEffect
{
  private IntervalTimer durationTimer;
  private IntervalTimer frequencyTimer;

  protected TimedStatusEffect(TimeSpan duration, TimeSpan frequency)
  {
    durationTimer = new IntervalTimer(duration);
    frequencyTimer = new IntervalTimer(frequency);
  }

  public sealed override Transition OnEffectUpdate(object target, TimeDelta deltaTime)
  {
    if (frequencyTimer.Tick(deltaTime))
    {
      OnEffectTick(target, deltaTime);

      frequencyTimer.Reset();
    }

    if (durationTimer.Tick(deltaTime))
    {
      return Transition.Remove;
    }

    return Transition.Continue;
  }

  protected abstract void OnEffectTick(object target, TimeDelta deltaTime);
}
