using Isaac.Core.Effects;
using Surreal.Actions;
using Surreal.Combat;
using Surreal.Persistence;

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
      new AddAttribute(Attributes.Health, 1),
      new ApplyDamage(new Damage(10, DamageType.Standard)),
      new AddStatusEffect(new FrozenStatusEffect.Template()),
    };

    await actions.ExecuteAsync(new ActionContext(player, player.PropertyBag));

    scene.Update(1.Seconds());
    scene.Update(1.Seconds());
    scene.Update(1.Seconds());
    scene.Update(1.Seconds());

    var persistenceContext = new PersistenceContext(new InMemoryPersistenceStore());

    player.Persist(persistenceContext);
    player.Resume(persistenceContext);

    player.Destroy();

    scene.Update(16.Milliseconds());
  }
}
