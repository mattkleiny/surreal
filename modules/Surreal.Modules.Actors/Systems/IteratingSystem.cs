using Surreal.Components;
using Surreal.Timing;

namespace Surreal.Systems;

/// <summary>A simple <see cref="ComponentSystem"/> that iterates components linearly.</summary>
public abstract class IteratingSystem : ComponentSystem
{
  private readonly HashSet<ActorId> actorIds = new();

  protected IteratingSystem(IComponentSystemContext context, ComponentMask mask)
    : base(context)
  {
    Mask = mask;

    context.ComponentAdded   += OnComponentAdded;
    context.ComponentRemoved += OnComponentRemoved;
  }

  public ComponentMask Mask { get; }

  public sealed override void OnInput(DeltaTime time)
  {
    OnBeginInput(time);

    foreach (var actor in actorIds)
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

    foreach (var actor in actorIds)
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

    foreach (var actor in actorIds)
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

  private void OnComponentAdded(ActorId id, ComponentType type)
  {
    if (Mask.Contains(type))
    {
      actorIds.Add(id);
    }
  }

  private void OnComponentRemoved(ActorId id, ComponentType type)
  {
    if (Mask.Contains(type))
    {
      actorIds.Remove(id);
    }
  }
}
