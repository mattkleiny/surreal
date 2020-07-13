using System.Numerics;

namespace Surreal.Framework.Scenes.Actors {
  public class Actor3D : Actor {
    public Vector3    Position { get; set; } = Vector3.Zero;
    public Vector3    Scale    { get; set; } = new Vector3(1, 1, 1);
    public Quaternion Rotation { get; set; } = Quaternion.Identity;

    protected override void ComputeModelToWorld(out Matrix4x4 modelToWorld) {
      var translation = Matrix4x4.CreateTranslation(Position.X, Position.Y, Position.Z);
      var scale       = Matrix4x4.CreateScale(Scale.X, Scale.Y, Scale.Z);
      var rotation    = Matrix4x4.CreateFromQuaternion(Rotation);

      modelToWorld = translation * rotation * scale;
    }
  }
}