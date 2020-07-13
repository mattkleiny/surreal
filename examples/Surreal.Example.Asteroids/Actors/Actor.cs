using System.Numerics;

namespace Asteroids.Actors {
  public class Actor : Surreal.Framework.Scenes.Actors.Actor {
    public Vector2 Position { get; set; } = Vector2.Zero;

    protected override void ComputeModelToWorld(out Matrix4x4 modelToWorld) {
      modelToWorld = Matrix4x4.CreateTranslation(Position.X, Position.Y, 0f);
    }
  }
}