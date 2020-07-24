using System.Numerics;
using Surreal.Framework.Parameters;
using Surreal.Mathematics;

namespace Isaac.Core {
  public abstract class Actor : Surreal.Framework.Scenes.Actors.Actor {
    public virtual Vector2Parameter Position { get; } = new Vector2Parameter(Vector2.Zero);
    public virtual AngleParameter   Rotation { get; } = new AngleParameter(Angle.Zero);

    protected override void ComputeModelToWorld(out Matrix4x4 modelToWorld) {
      var translation = Matrix4x4.CreateTranslation(Position.Value.X, Position.Value.Y, 0f);
      var rotation    = Matrix4x4.CreateRotationZ(Rotation.Value.Radians);

      modelToWorld = rotation * translation;
    }
  }
}