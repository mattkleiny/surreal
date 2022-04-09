using Surreal.Components;

namespace Surreal.Systems;

/// <summary>Base class for any <see cref="ISceneSystem"/> implementation that monitors components..</summary>
public abstract class ComponentSystem : SceneSystem
{
  protected ComponentSystem(ComponentMask mask)
  {
    Mask = mask;
  }

  public ComponentMask    Mask     { get; }
  public HashSet<ActorId> ActorIds { get; } = new();

  // TODO: find a way to fire these events?

  protected internal virtual void OnComponentAdded(ActorId id, ComponentType type)
  {
    if (Mask.Contains(type))
    {
      ActorIds.Add(id);
    }
  }

  protected internal virtual void OnComponentRemoved(ActorId id, ComponentType type)
  {
    if (Mask.Contains(type))
    {
      ActorIds.Remove(id);
    }
  }
}
