using System.Numerics;
using Surreal.Framework.Parameters;

namespace Isaac.Core {
  public abstract class Actor : Surreal.Framework.Scenes.Actors.Actor {
    public virtual Parameter<Vector2> Position { get; } = new Vector2Parameter(Vector2.Zero);

    protected override void ComputeModelToWorld(out Matrix4x4 modelToWorld) {
      modelToWorld = Matrix4x4.CreateTranslation(Position.Value.X, Position.Value.Y, 0f);
    }
  }
}