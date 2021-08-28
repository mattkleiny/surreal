using Surreal.Mechanics.Tactical.Effects;
using Surreal.Timing;

namespace Isaac.Mechanics.Effects
{
  public interface IFreezable
  {
    void OnFrozen();
    void OnUnfrozen();
  }

  public record FrozenEffect() : TimedStatusEffect(Frequency: 1.Seconds(), Duration: 10.Seconds())
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
      base.OnRemoved(owner);

      if (owner is IFreezable freezable)
      {
        freezable.OnUnfrozen();
      }
    }
  }
}
