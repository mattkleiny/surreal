using Surreal.Effects;
using Surreal.Objects;
using Surreal.Utilities;

namespace Isaac.Core.Effects;

[EditorDescription(
  Name = "DoT",
  Category = "Status Effects",
  Description = "Applies a damage-over-time effect to an object"
)]
public sealed class PoisonStatusEffect : TimedStatusEffect
{
  private readonly Damage damage;

  public PoisonStatusEffect(TimeSpan duration, TimeSpan frequency, Damage damage)
    : base(duration, frequency)
  {
    this.damage = damage;
  }

  public override StatusEffectType Type => StatusEffectTypes.Poison;

  protected override void OnEffectTick(object owner, DeltaTime deltaTime)
  {
    if (owner is IDamageReceiver receiver)
    {
      damage.ApplyTo(receiver);
    }
  }

  [Template(typeof(PoisonStatusEffect))]
  public sealed record Template : ITemplate<PoisonStatusEffect>
  {
    public TimeSpan Duration  { get; init; } = 10.Seconds();
    public TimeSpan Frequency { get; init; } = 1.Seconds();
    public Damage   Damage    { get; init; } = new(1, DamageTypes.Poison);

    public PoisonStatusEffect Create()
    {
      return new PoisonStatusEffect(Duration, Frequency, Damage);
    }
  }
}
