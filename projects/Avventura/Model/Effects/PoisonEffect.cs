using Avventura.Model.Actors;

namespace Avventura.Model.Effects {
  public sealed class PoisonEffect : TimedEffect {
    private readonly Damage damage;

    public PoisonEffect(Damage damage) {
      this.damage = damage;
    }

    protected override void NextTick(Character character) {
      character.Attributes.Health.Value -= damage.Amount;
    }
  }
}