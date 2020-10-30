using Isaac.Core.Brains;
using Surreal.Framework.Parameters;
using Surreal.Graphics.Textures;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Mobs {
  public sealed class Monster : Mob {
    public Parameter<Brain> Brain { get; } = new(Brains.Brain.Null);

    public Monster(TextureRegion sprite)
        : base(sprite) {
    }

    public override void Update(DeltaTime deltaTime) {
      base.Update(deltaTime);

      Brain.Value.Think(deltaTime, this);
    }
  }
}