using Surreal.Components;

namespace Surreal.Systems;

/// <summary>A listener for changes in component composition.</summary>
public delegate void ComponentChangeListener(ActorId id, ComponentType type);

/// <summary>Context for component system operations.</summary>
public interface IComponentSystemContext
{
  event ComponentChangeListener ComponentAdded;
  event ComponentChangeListener ComponentRemoved;
}

/// <summary>Base class for any <see cref="ISceneSystem"/> implementation that monitors components..</summary>
public abstract class ComponentSystem : SceneSystem
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

  protected virtual void OnComponentAdded(ActorId id, ComponentType type)
  {
    if (Mask.Contains(type))
    {
      ActorIds.Add(id);
    }
  }

  protected virtual void OnComponentRemoved(ActorId id, ComponentType type)
  {
    if (Mask.Contains(type))
    {
      ActorIds.Remove(id);
    }
  }
}
