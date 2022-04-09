using Isaac.Core.Actors;

namespace Isaac.Core.Effects;

public class AttributeBoostEffectTests
{
  [Test]
  public void it_should_boost_attribute_of_character()
  {
    var character = new Character
    {
      Health = 50,
    };

    character.StatusEffects.Add(new AttributeBoostEffect(10.Seconds(), AttributeTypes.Health, 20));
    character.Health.Should().Be(70);

    character.StatusEffects.Update(10.Seconds());
    character.Health.Should().Be(50);
  }
}
