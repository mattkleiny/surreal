using Surreal.Aspects;
using Surreal.Timing;

namespace Surreal.Systems;

/// <summary>A simple <see cref="ComponentSystem"/> that iterates components linearly.</summary>
public abstract class IteratingSystem : ComponentSystem
{
  protected IteratingSystem(Aspect aspect)
    : base(aspect)
  {
  }

  public sealed override void OnInput(TimeDelta deltaTime)
  {
    OnBeginInput(deltaTime);

    foreach (var actor in ActorIds)
    {
      OnInput(deltaTime, actor);
    }

    OnEndInput(deltaTime);
  }

  protected virtual void OnBeginInput(TimeDelta time)
  {
  }

  protected virtual void OnInput(TimeDelta deltaTime, ActorId actor)
  {
  }

  protected virtual void OnEndInput(TimeDelta time)
  {
  }

  public sealed override void OnUpdate(TimeDelta deltaTime)
  {
    OnBeginUpdate(deltaTime);

    foreach (var actor in ActorIds)
    {
      OnUpdate(deltaTime, actor);
    }

    OnEndUpdate(deltaTime);
  }

  protected virtual void OnBeginUpdate(TimeDelta time)
  {
  }

  protected virtual void OnUpdate(TimeDelta deltaTime, ActorId actor)
  {
  }

  protected virtual void OnEndUpdate(TimeDelta time)
  {
  }

  public sealed override void OnDraw(TimeDelta deltaTime)
  {
    OnBeginDraw(deltaTime);

    foreach (var actor in ActorIds)
    {
      OnDraw(deltaTime, actor);
    }

    OnEndDraw(deltaTime);
  }

  protected virtual void OnBeginDraw(TimeDelta time)
  {
  }

  protected virtual void OnDraw(TimeDelta deltaTime, ActorId actor)
  {
  }

  protected virtual void OnEndDraw(TimeDelta time)
  {
  }
}
