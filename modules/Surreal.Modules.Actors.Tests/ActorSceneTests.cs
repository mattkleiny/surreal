using Surreal.Systems;
using Surreal.Timing;

namespace Surreal;

public class ActorSceneTests
{
  [Test]
  public void it_should_spawn_actors()
  {
    var scene = new ActorScene();
    var actor = Substitute.For<Actor>();

    scene.Spawn(actor);

    actor.Received(1).OnAwake();
    Assert.AreEqual(ActorStatus.Active, actor.Status);
  }

  [Test]
  public void it_should_apply_input_to_actors()
  {
    var scene     = new ActorScene();
    var actor     = Substitute.For<Actor>();
    var deltaTime = 16.Milliseconds();

    scene.Spawn(actor);
    scene.Input(deltaTime);

    actor.Received(1).OnInput(deltaTime);
  }

  [Test, AutoFixture]
  public void it_should_apply_input_to_systems(IComponentSystem system)
  {
    var scene     = new ActorScene();
    var deltaTime = 16.Milliseconds();

    scene.AddSystem(system);
    scene.Input(deltaTime);

    system.Received(1).OnInput(deltaTime);
  }

  [Test]
  public void it_should_apply_update_to_actors()
  {
    var scene     = new ActorScene();
    var actor     = Substitute.For<Actor>();
    var deltaTime = 16.Milliseconds();

    scene.Spawn(actor);
    scene.Update(deltaTime);

    actor.Received(1).OnUpdate(deltaTime);
  }

  [Test, AutoFixture]
  public void it_should_apply_update_to_systems(IComponentSystem system)
  {
    var scene     = new ActorScene();
    var deltaTime = 16.Milliseconds();

    scene.AddSystem(system);
    scene.Update(deltaTime);

    system.Received(1).OnUpdate(deltaTime);
  }

  [Test]
  public void it_should_apply_draw_to_actors()
  {
    var scene     = new ActorScene();
    var actor     = Substitute.For<Actor>();
    var deltaTime = 16.Milliseconds();

    scene.Spawn(actor);
    scene.Draw(deltaTime);

    actor.Received(1).OnDraw(deltaTime);
  }

  [Test, AutoFixture]
  public void it_should_apply_draw_to_systems(IComponentSystem system)
  {
    var scene     = new ActorScene();
    var deltaTime = 16.Milliseconds();

    scene.AddSystem(system);
    scene.Draw(deltaTime);

    system.Received(1).OnDraw(deltaTime);
  }

  [Test]
  public void it_should_destroy_actors_after_next_tick()
  {
    var scene     = new ActorScene();
    var actor     = Substitute.For<Actor>();
    var deltaTime = 16.Milliseconds();

    scene.Spawn(actor);
    actor.Destroy();

    Assert.AreEqual(ActorStatus.Destroyed, actor.Status);
    actor.Received(0).OnDestroy();

    scene.Update(deltaTime);
    actor.Received(1).OnDestroy();
  }
}
