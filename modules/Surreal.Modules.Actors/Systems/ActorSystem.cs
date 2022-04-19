using Surreal.Timing;

namespace Surreal.Systems;

/// <summary>Base class for any system that works directly on actors.</summary>
public abstract class ActorSystem : IDisposable
{
  public ActorScene? Scene { get; private set; }

  public virtual void OnAddedToScene(ActorScene scene)
  {
    Scene = scene;
  }

  public virtual void OnRemovedFromScene(ActorScene scene)
  {
    Scene = null;
  }

  public virtual void OnBeginFrame(DeltaTime deltaTime)
  {
  }

  public virtual void OnInput(DeltaTime deltaTime, Actor actor)
  {
  }

  public virtual void OnUpdate(DeltaTime deltaTime, Actor actor)
  {
  }

  public virtual void OnDraw(DeltaTime deltaTime, Actor actor)
  {
  }

  public virtual void OnEndFrame(DeltaTime deltaTime)
  {
  }

  public virtual void Dispose()
  {
  }
}
