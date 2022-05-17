using Surreal.Timing;

namespace Surreal.Systems.Effects;

/// <summary>A <see cref="StatusEffect"/> that persists over a duration but does not tick.</summary>
public abstract class PermanentStatusEffect : StatusEffect
{
  private IntervalTimer durationTimer;

  protected PermanentStatusEffect(TimeSpan duration)
  {
    durationTimer = new IntervalTimer(duration);
  }

  public sealed override Transition OnEffectUpdate(object target, TimeDelta deltaTime)
  {
    if (durationTimer.Tick(deltaTime))
    {
      return Transition.Remove;
    }

    return Transition.Continue;
  }
}
