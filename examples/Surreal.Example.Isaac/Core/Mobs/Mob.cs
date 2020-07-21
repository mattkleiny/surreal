using System.Numerics;
using Surreal.Framework.Parameters;
using Surreal.Mathematics;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Mobs {
  public abstract class Mob : Actor {
    public virtual Parameter<int>     Health    { get; } = new ClampedIntParameter(10, Range.Of(0, 100));
    public virtual Parameter<float>   Speed     { get; } = new ClampedFloatParameter(4f, Range.Of(0f, 100f));
    public virtual Parameter<Vector2> Direction { get; } = new Vector2Parameter(Vector2.Zero);

    public override void Update(DeltaTime deltaTime) {
      base.Update(deltaTime);

      Position.Value += Direction.Value * Speed.Value * deltaTime;
    }
  }
}