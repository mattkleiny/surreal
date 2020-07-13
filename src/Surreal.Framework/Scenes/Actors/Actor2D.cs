using System.Numerics;

namespace Surreal.Framework.Scenes.Actors {
  public class Actor2D : Actor {
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 Scale    { get; set; } = new Vector2(1, 1);
    public float   Rotation { get; set; } = 0f;

    protected override void ComputeModelToWorld(out Matrix4x4 modelToWorld) {
      var translation = Matrix4x4.CreateTranslation(Position.X, Position.Y, 0f);
      var scale       = Matrix4x4.CreateScale(Scale.X, Scale.Y, 1f);
      var rotation    = Matrix4x4.CreateRotationZ(Rotation);

      modelToWorld = translation * rotation * scale;
    }
  }
}