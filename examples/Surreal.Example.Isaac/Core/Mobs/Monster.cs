using Isaac.Core.Brains;
using Surreal.Framework.Parameters;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Mobs {
  public sealed class Monster : Mob {
    public Monster(Brain brain) {
      Brain = new Parameter<Brain>(brain);
    }

    public Parameter<Brain> Brain { get; }

    public override void Update(DeltaTime deltaTime) {
      base.Update(deltaTime);

      Brain.Value.Think(deltaTime, this);
    }
  }
}