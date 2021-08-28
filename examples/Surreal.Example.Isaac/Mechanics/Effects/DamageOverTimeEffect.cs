using System;
using Surreal.Mechanics.Tactical;
using Surreal.Mechanics.Tactical.Effects;
using Surreal.Timing;

namespace Isaac.Mechanics.Effects
{
  public record DamageOverTimeEffect(Damage Damage, TimeSpan Frequency, TimeSpan Duration) : TimedStatusEffect(Frequency, Duration)
  {
    protected override void OnTick(object owner, DeltaTime deltaTime)
    {
      if (owner is IDamageReceiver receiver)
      {
        var damage = Damage.Calculate(Damage, owner, owner);

        receiver.ReceiveDamage(owner, damage);
      }
    }
  }
}
