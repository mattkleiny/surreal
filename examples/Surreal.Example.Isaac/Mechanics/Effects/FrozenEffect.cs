using System;
using Surreal.Mechanics.Tactical.Effects;
using Surreal.Objects;
using Surreal.Timing;

namespace Isaac.Mechanics.Effects
{
  public interface IFreezable
  {
    void OnFrozen();
    void OnUnfrozen();
  }

  [Template(typeof(FrozenEffectTemplate))]
  public record FrozenEffect(TimeSpan Frequency, TimeSpan Duration) : TimedStatusEffect(Frequency, Duration)
  {
    public override void OnAdded(object owner)
    {
      base.OnAdded(owner);

      if (owner is IFreezable freezable)
      {
        freezable.OnFrozen();
      }
    }

    public override void OnRemoved(object owner)
    {
      if (owner is IFreezable freezable)
      {
        freezable.OnUnfrozen();
      }

      base.OnRemoved(owner);
    }
  }

  public class FrozenEffectTemplate : ITemplate<FrozenEffect>
  {
    public TimeSpan Frequency { get; set; } = 1.Seconds();
    public TimeSpan Duration  { get; set; } = 10.Seconds();

    public FrozenEffect Create() => new(Frequency, Duration);
  }
}
