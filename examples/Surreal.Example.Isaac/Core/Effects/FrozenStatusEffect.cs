using Surreal.Effects;
using Surreal.Objects;
using Surreal.Utilities;

namespace Isaac.Core.Effects;

/// <summary>Freezes an object in place.</summary>
[EditorDescription(
  Name = "Frozen",
  Category = "Status Effects",
  Description = "Freezes an object in-place, preventing it's movement"
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
  public sealed record Template : ITemplate<FrozenStatusEffect>
  {
    public TimeSpan Duration { get; init; } = 4.Seconds();

    public FrozenStatusEffect Create()
    {
      return new FrozenStatusEffect(Duration);
    }
  }
}
