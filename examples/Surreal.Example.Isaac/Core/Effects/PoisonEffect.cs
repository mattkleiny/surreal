using Surreal.Combat;
using Surreal.Combat.Effects;
using Surreal.Objects;
using Surreal.Utilities;

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

  protected override void OnEffectTick(object target, DeltaTime deltaTime)
  {
    if (target is IDamageReceiver receiver)
    {
      damage.ApplyTo(receiver);
    }
  }

  [Template(typeof(PoisonEffect))]
  public sealed record Template : ITemplate<PoisonEffect>
  {
    [Bind] public TimeSpan Duration  { get; init; } = 10.Seconds();
    [Bind] public TimeSpan Frequency { get; init; } = 1.Seconds();
    [Bind] public Damage   Damage    { get; init; } = new(1, DamageTypes.Poison);

    public PoisonEffect Create() => new(Duration, Frequency, Damage);
  }
}
