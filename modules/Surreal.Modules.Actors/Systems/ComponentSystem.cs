using Surreal.Components;
using Surreal.Timing;

namespace Surreal.Systems;

/// <summary>A listener for changes in component composition.</summary>
public delegate void ComponentChangeListener(ActorId id, ComponentType type);

/// <summary>Represents a component system, capable of operating on components.</summary>
public interface IComponentSystem
{
  void OnInput(DeltaTime time);
  void OnUpdate(DeltaTime time);
  void OnDraw(DeltaTime time);
}

/// <summary>Context for component system operations.</summary>
public interface IComponentSystemContext
{
  event ComponentChangeListener ComponentAdded;
  event ComponentChangeListener ComponentRemoved;
}

/// <summary>Base class for any <see cref="IComponentSystem"/> implementation.</summary>
public abstract class ComponentSystem : IComponentSystem
{
  protected ComponentSystem(IComponentSystemContext context)
    : this(context, ComponentMask.Empty)
  {
  }

  protected ComponentSystem(IComponentSystemContext context, ComponentMask mask)
  {
    Context = context;
    Mask = mask;

    if (mask != ComponentMask.Empty)
    {
      context.ComponentAdded += OnComponentAdded;
      context.ComponentRemoved += OnComponentRemoved;
    }
  }

  public IComponentSystemContext Context  { get; }
  public ComponentMask           Mask     { get; }
  public HashSet<ActorId>        ActorIds { get; } = new();

  public virtual void OnInput(DeltaTime time)
  {
  }

  public virtual void OnUpdate(DeltaTime time)
  {
  }

  public virtual void OnDraw(DeltaTime time)
  {
  }

  private void OnComponentAdded(ActorId id, ComponentType type)
  {
    if (Mask.Contains(type))
    {
      ActorIds.Add(id);
    }
  }

  private void OnComponentRemoved(ActorId id, ComponentType type)
  {
    if (Mask.Contains(type))
    {
      ActorIds.Remove(id);
    }
  }
}
