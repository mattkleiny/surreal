using System;
using System.Numerics;
using Surreal.Framework.Scenes.Actors;
using Surreal.Mathematics.Linear;

namespace Prelude.Core.Actors
{
  public class PreludeActor : Actor2D
  {
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
  }
}
