using Isaac.Core.Effects;
using Surreal.Persistence;
using Surreal.Scripting;

namespace Isaac.Core.Actors;

public class PlayerTests
{
  [Test]
  public async Task it_should_work_properly()
  {
    using var scene = new ActorScene();

    var player = new Player
    {
      Health = 100,
      Bombs = 99,
      Coins = 12,
      Range = 4,
      MoveSpeed = 2,
      AttackSpeed = 3,
    };

    scene.Spawn(player);

    var actions = new ActionList
    {
      new AddAttribute(AttributeTypes.Health, 1),
      new ApplyDamage(new Damage(10, DamageTypes.Standard)),
      new AddStatusEffect(new FrozenStatusEffect.Template
      {
        Duration = 4.Seconds()
      }),
      new AddStatusEffect(new PoisonStatusEffect.Template
      {
        Damage = new Damage(10, DamageTypes.Poison)
      }),
    };

    await actions.ExecuteAsync(new ActionContext(player, player.Properties));

    scene.Update(1.Seconds());
    scene.Update(1.Seconds());
    scene.Update(1.Seconds());
    scene.Update(1.Seconds());

    var context = new PersistenceContext(new InMemoryPersistenceStore());

    player.Persist(context);

    player.Health = 0;
    player.Bombs = 0;

    player.Resume(context);

    player.Destroy();

    scene.Update(16.Milliseconds());
  }
}
