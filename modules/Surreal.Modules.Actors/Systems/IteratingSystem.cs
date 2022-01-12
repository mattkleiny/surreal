using Surreal.Components;
using Surreal.Timing;

namespace Surreal.Systems;

/// <summary>A simple <see cref="ComponentSystem"/> that iterates components linearly.</summary>
public abstract class IteratingSystem : ComponentSystem
{
  protected IteratingSystem(IComponentSystemContext context)
    : this(context, ComponentMask.Empty)
  {
  }

  protected IteratingSystem(IComponentSystemContext context, ComponentMask mask)
    : base(context, mask)
  {
  }

  public sealed override void OnInput(DeltaTime time)
  {
    OnBeginInput(time);

    foreach (var actor in ActorIds)
    {
      OnInput(time, actor);
    }

    OnEndInput(time);
  }

  protected virtual void OnBeginInput(DeltaTime time)
  {
  }

  protected virtual void OnInput(DeltaTime time, ActorId actor)
  {
  }

  protected virtual void OnEndInput(DeltaTime time)
  {
  }

  public sealed override void OnUpdate(DeltaTime time)
  {
    OnBeginUpdate(time);

    foreach (var actor in ActorIds)
    {
      OnUpdate(time, actor);
    }

    OnEndUpdate(time);
  }

  protected virtual void OnBeginUpdate(DeltaTime time)
  {
  }

  protected virtual void OnUpdate(DeltaTime time, ActorId actor)
  {
  }

  protected virtual void OnEndUpdate(DeltaTime time)
  {
  }

  public sealed override void OnDraw(DeltaTime time)
  {
    OnBeginDraw(time);

    foreach (var actor in ActorIds)
    {
      OnDraw(time, actor);
    }

    OnEndDraw(time);
  }

  protected virtual void OnBeginDraw(DeltaTime time)
  {
  }

  protected virtual void OnDraw(DeltaTime time, ActorId actor)
  {
  }

  protected virtual void OnEndDraw(DeltaTime time)
  {
  }
}
