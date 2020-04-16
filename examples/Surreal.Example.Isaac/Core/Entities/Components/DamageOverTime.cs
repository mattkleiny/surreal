using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Timing;

namespace Isaac.Core.Entities.Components
{
  public struct DamageOverTime : IComponent
  {
    public Damage        Damage;
    public EmbeddedTimer Frequency;
    public EmbeddedTimer Lifetime;
  }
}
