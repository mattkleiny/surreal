using Isaac.Core.Brains;
using Surreal.Framework.Parameters;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Mobs {
  public sealed class Monster : Mob {
    public Parameter<Brain> Brain { get; } = new Parameter<Brain>(SimpletonBrain.Instance);

    public override void Update(DeltaTime deltaTime) {
      base.Update(deltaTime);

      Brain.Value.Think(deltaTime, this);
    }
  }
}