using Isaac.Core.Effects;
using Surreal.Actions;
using Surreal.Combat;

namespace Isaac.Core.Actors;

public class CharacterTests
{
  [Test]
  public async Task it_should_work_properly()
  {
    using var scene = new ActorScene();

    var character = new Character(scene)
    {
      Health = 100,
      Bombs = 99,
      Coins = 12,
      Range = 4,
      MoveSpeed = 2,
      AttackSpeed = 3,
    };

    character.Bombs.Should().Be(99);

    var actions = new ActionList
    {
      new AddAttribute(Attributes.Health, 1),
      new ApplyDamage(new Damage(10, DamageType.Standard)),
      new AddStatusEffect(new FrozenStatusEffect.Template(32.Milliseconds())),
    };

    await actions.ExecuteAsync(new ActionContext(character, character.PropertyBag));

    scene.Spawn(character);

    scene.Update(16.Milliseconds());
    scene.Update(16.Milliseconds());
    scene.Update(16.Milliseconds());
    scene.Update(16.Milliseconds());

    scene.Despawn(character);

    scene.Update(16.Milliseconds());
  }
}
