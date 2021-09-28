using System;
using Surreal.Mechanics.Tactical.Combat;
using Surreal.Mechanics.Tactical.Effects;
using Surreal.Objects;
using Surreal.Timing;

namespace Isaac.Mechanics.Effects
{
  [Template(typeof(DamageEffectTemplate))]
  public record DamageEffect(Damage Damage, TimeSpan Frequency, TimeSpan Duration) : TimedStatusEffect(Frequency, Duration)
  {
    protected override void OnTick(object owner, DeltaTime deltaTime)
    {
    }
  }

  public class DamageEffectTemplate : ITemplate<DamageEffect>
  {
    public Damage   Damage    { get; set; } = new(1, DamageTypes.Physical);
    public TimeSpan Frequency { get; set; } = 1.Seconds();
    public TimeSpan Duration  { get; set; } = 10.Seconds();

    public virtual DamageEffect Create()
    {
      return new DamageEffect(Damage, Frequency, Duration);
    }
  }
}
