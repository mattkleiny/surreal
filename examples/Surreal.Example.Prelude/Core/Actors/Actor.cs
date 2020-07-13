using System;
using System.Numerics;
using Surreal.Mathematics.Linear;

namespace Prelude.Core.Actors {
  public class Actor : Surreal.Framework.Scenes.Actors.Actor {
    public Vector2 Position  { get; set; } = Vector2.Zero;
    public float   Radius    { get; set; } = 0.25f;
    public Vector2 Velocity  { get; set; } = Vector2.Zero;
    public Vector2 Direction { get; set; } = new Vector2(0, 1);

    public float Speed        { get; set; } = 1f;
    public float TurningSpeed { get; set; } = MathF.PI;

    public Rectangle Bounds => new Rectangle(
        left: Position.X - Radius,
        top: Position.Y + Radius,
        right: Position.X + Radius,
        bottom: Position.Y - Radius
    );

    protected override void ComputeModelToWorld(out Matrix4x4 modelToWorld) {
      modelToWorld = Matrix4x4.CreateTranslation(Position.X, Position.Y, 0f);
    }
  }
}