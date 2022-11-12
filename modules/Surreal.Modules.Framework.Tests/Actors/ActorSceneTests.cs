using Surreal.Timing;

namespace Surreal.Actors;

public class ActorSceneTests
{
  [Test]
  public void it_should_spawn_actors()
  {
    var scene = new ActorScene();
    var actor = Substitute.For<Actor>();
    var deltaTime = 16.Milliseconds();

    scene.Spawn(actor);
    scene.BeginFrame(deltaTime);

    actor.Received(1).OnAwake();
    actor.Status.Should().Be(ActorStatus.Active);
  }

  [Test]
  public void it_should_apply_input_to_actors()
  {
    var scene = new ActorScene();
    var actor = Substitute.For<Actor>();
    var deltaTime = 16.Milliseconds();

    scene.Spawn(actor);
    scene.BeginFrame(deltaTime);
    scene.Input(deltaTime);

    actor.Received(1).OnInput(deltaTime);
  }

  [Test]
  public void it_should_apply_update_to_actors()
  {
    var scene = new ActorScene();
    var actor = Substitute.For<Actor>();
    var deltaTime = 16.Milliseconds();

    scene.Spawn(actor);
    scene.BeginFrame(deltaTime);
    scene.Update(deltaTime);

    actor.Received(1).OnUpdate(deltaTime);
  }

  [Test]
  public void it_should_apply_draw_to_actors()
  {
    var scene = new ActorScene();
    var actor = Substitute.For<Actor>();
    var deltaTime = 16.Milliseconds();

    scene.Spawn(actor);
    scene.BeginFrame(deltaTime);
    scene.Draw(deltaTime);

    actor.Received(1).OnDraw(deltaTime);
  }

  [Test]
  public void it_should_destroy_actors_after_next_tick()
  {
    var scene = new ActorScene();
    var actor = Substitute.For<Actor>();
    var deltaTime = 16.Milliseconds();

    scene.Spawn(actor);
    scene.BeginFrame(deltaTime);
    actor.Destroy();

    actor.Status.Should().Be(ActorStatus.Destroyed);
    actor.Received(0).OnDestroy();

    scene.EndFrame(deltaTime);
    actor.Received(1).OnDestroy();
  }
}



