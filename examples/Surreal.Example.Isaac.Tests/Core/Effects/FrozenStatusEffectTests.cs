using Isaac.Core.Actors;

namespace Isaac.Core.Effects;

public class FrozenStatusEffectTests
{
  [Test]
  public void it_should_freeze_character()
  {
    var character = new Character();
    character.LocomotionState.Should().Be(LocomotionState.Normal);

    character.StatusEffects.Add(new FrozenStatusEffect(1.Seconds()));
    character.LocomotionState.Should().Be(LocomotionState.Stuck);

    character.StatusEffects.Update(1.Seconds());
    character.LocomotionState.Should().Be(LocomotionState.Normal);
  }
}
