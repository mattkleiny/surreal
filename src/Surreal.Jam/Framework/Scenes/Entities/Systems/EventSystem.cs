using Surreal.Framework.Events;

namespace Surreal.Framework.Scenes.Entities.Systems
{
  public sealed class EventSystem : EntitySystem
  {
    public EventSystem()
      : this(new EventBus())
    {
    }

    public EventSystem(IEventBus eventBus)
    {
      EventBus = eventBus;
    }

    public IEventBus EventBus { get; }

    public override void Initialize(EntityScene scene)
    {
      base.Initialize(scene);

      foreach (var system in scene.Systems)
      {
        EventBus.RegisterListeners(system);
      }

      scene.SystemAdded += OnSystemAdded;
    }

    public override void Dispose()
    {
      if (World != null)
      {
        World.SystemAdded -= OnSystemAdded;
      }

      base.Dispose();
    }

    private void OnSystemAdded(IEntitySystem system)
    {
      EventBus.RegisterListeners(system);
    }
  }
}
