using Surreal.Aspects;
using Surreal.Timing;

namespace Surreal.Systems;

/// <summary>A simple <see cref="ComponentSystem"/> that iterates components linearly.</summary>
public abstract class IteratingSystem : ComponentSystem
{
  protected IteratingSystem(Aspect mask)
    : base(mask)
  {
  }

  public sealed override void OnInput(DeltaTime deltaTime)
  {
    OnBeginInput(deltaTime);

    foreach (var actor in ActorIds)
    {
      OnInput(deltaTime, actor);
    }

    OnEndInput(deltaTime);
  }

  protected virtual void OnBeginInput(DeltaTime time)
  {
  }

  protected virtual void OnInput(DeltaTime deltaTime, ActorId actor)
  {
  }

  protected virtual void OnEndInput(DeltaTime time)
  {
  }

  public sealed override void OnUpdate(DeltaTime deltaTime)
  {
    OnBeginUpdate(deltaTime);

    foreach (var actor in ActorIds)
    {
      OnUpdate(deltaTime, actor);
    }

    OnEndUpdate(deltaTime);
  }

  protected virtual void OnBeginUpdate(DeltaTime time)
  {
  }

  protected virtual void OnUpdate(DeltaTime deltaTime, ActorId actor)
  {
  }

  protected virtual void OnEndUpdate(DeltaTime time)
  {
  }

  public sealed override void OnDraw(DeltaTime deltaTime)
  {
    OnBeginDraw(deltaTime);

    foreach (var actor in ActorIds)
    {
      OnDraw(deltaTime, actor);
    }

    OnEndDraw(deltaTime);
  }

  protected virtual void OnBeginDraw(DeltaTime time)
  {
  }

  protected virtual void OnDraw(DeltaTime deltaTime, ActorId actor)
  {
  }

  protected virtual void OnEndDraw(DeltaTime time)
  {
  }
}
