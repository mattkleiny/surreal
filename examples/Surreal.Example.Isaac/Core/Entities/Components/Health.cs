using System.Diagnostics;
using Surreal.Framework.Scenes.Entities.Components;

namespace Isaac.Core.Entities.Components {
  [DebuggerDisplay("Health {Life}")]
  public struct Health : IComponent {
    public int Life;

    public void Apply(Damage damage) {
      // TODO: apply damage types/etc

      Life -= damage.Amount;
    }
  }
}