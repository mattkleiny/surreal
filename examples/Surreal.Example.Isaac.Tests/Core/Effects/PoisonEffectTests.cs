using Isaac.Core.Actors;

namespace Isaac.Core.Effects;

public class PoisonEffectTests
{
  [Test]
  public void it_should_damage_character_over_time()
  {
    var character = new Character
    {
      Health = 99,
    };

    character.StatusEffects.Add(new PoisonEffect(
      duration: 4.Seconds(),
      frequency: 1.Seconds(),
      damage: new Damage(10, DamageTypes.Poison)
    ));

    character.StatusEffects.Update(1.Seconds());
    character.Health.Should().Be(89);

    character.StatusEffects.Update(1.Seconds());
    character.Health.Should().Be(79);

    character.StatusEffects.Update(1.Seconds());
    character.Health.Should().Be(69);

    character.StatusEffects.Update(1.Seconds());
    character.Health.Should().Be(59);

    // removed
    character.StatusEffects.Update(1.Seconds());
    character.Health.Should().Be(59);
  }
}
