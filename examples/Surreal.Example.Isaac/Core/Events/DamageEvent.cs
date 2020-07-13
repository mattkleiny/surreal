using Isaac.Core.Entities;
using Surreal.Framework.Scenes.Entities;

namespace Isaac.Core.Events {
  public readonly struct DamageEvent {
    public Entity? Source { get; }
    public Entity  Target { get; }
    public Damage  Damage { get; }

    public DamageEvent(Entity? source, Entity target, Damage damage) {
      Source = source;
      Target = target;
      Damage = damage;
    }
  }
}