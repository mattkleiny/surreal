using Surreal.Framework.Scenes.Entities;

namespace Isaac.Core.Events
{
  public readonly struct DiedEvent
  {
    public Entity Target { get; }

    public DiedEvent(Entity target)
    {
      Target = target;
    }
  }
}
