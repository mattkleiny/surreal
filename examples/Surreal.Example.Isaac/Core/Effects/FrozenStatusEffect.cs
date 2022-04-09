using Surreal.Effects;
using Surreal.Objects;
using Surreal.Utilities;

namespace Isaac.Core.Effects;

/// <summary>Freezes an object in place.</summary>
[Export(
  Category = "Status Effects",
  Description = "Freezes an object in-place"
)]
public sealed class FrozenStatusEffect : StatusEffect
{
  private IntervalTimer timer;

  public FrozenStatusEffect(TimeSpan duration)
  {
    timer = new IntervalTimer(duration);
  }

  public override Transition OnEffectUpdate(object owner, DeltaTime deltaTime)
  {
    if (timer.Tick(deltaTime))
    {
      return Transition.Remove;
    }

    return Transition.Continue;
  }

  [Template(typeof(FrozenStatusEffect))]
  public sealed record Template(TimeSpan Duration) : ITemplate<FrozenStatusEffect>
  {
    public FrozenStatusEffect Create()
    {
      return new FrozenStatusEffect(Duration);
    }
  }
}
