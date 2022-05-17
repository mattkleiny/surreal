using Surreal.Systems;
using Surreal.Systems.Effects;

namespace Isaac.Core.Effects;

[EditorDescription(
  Name = "DoT",
  Category = "Status Effects",
  Description = "Applies a damage-over-time effect to an object"
)]
public sealed class PoisonEffect : TimedStatusEffect
{
  private readonly Damage damage;

  public PoisonEffect(TimeSpan duration, TimeSpan frequency, Damage damage)
    : base(duration, frequency)
  {
    this.damage = damage;
  }

  public override StatusEffectType Type => StatusEffectTypes.Poison;

  protected override void OnEffectTick(object target, TimeDelta deltaTime)
  {
    if (target is IDamageReceiver receiver)
    {
      damage.ApplyTo(receiver);
    }
  }
}
