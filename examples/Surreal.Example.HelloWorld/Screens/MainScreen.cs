using System.Numerics;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Graphics;
using Surreal.Graphics.Cameras;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Curves;
using static Surreal.Mathematics.Maths;

namespace HelloWorld.Screens {
  public sealed class MainScreen : GameScreen<Game> {
    public MainScreen(Game game)
        : base(game) {
    }

    public OrthographicCamera Camera { get; } = new(viewportWidth: 256 / 4, viewportHeight: 144 / 4);

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();

      base.Input(time);
    }

    public override void Draw(GameTime time) {
      GeometryBatch.Begin(in Camera.ProjectionView);

      GeometryBatch.DrawLine(
          from: -V(256, 144) / 2f,
          to: V(256, 144) / 2f,
          color: Color.Red
      );

      GeometryBatch.DrawCircle(
          center: V(-4f, 4f),
          radius: 5f,
          color: Color.Blue,
          segments: 32
      );

      GeometryBatch.DrawSolidQuad(
          center: Vector2.Zero,
          size: V(5f, 10f),
          color: Color.Blue
      );

      GeometryBatch.DrawWireQuad(
          center: Vector2.Zero,
          size: V(5f, 10f) * 2f,
          color: Color.Green
      );

      GeometryBatch.DrawArc(
          center: Vector2.Zero,
          startAngle: 0f,
          endAngle: 90f,
          radius: 16f,
          color: Color.Brown,
          segments: 32
      );

      GeometryBatch.DrawSolidTriangle(
          a: Vector2.Zero,
          b: V(-16f, 16f),
          c: V(16f, 16f),
          color: Color.Magenta
      );

      GeometryBatch.DrawWireTriangle(
          a: Vector2.Zero,
          b: V(-16f, -16f),
          c: V(16f, -16f),
          color: Color.Green
      );

      GeometryBatch.DrawCurve(
          curve: new QuadraticBezierCurve(
              startPoint: V(-16f, -16f),
              controlPoint: V(-16, 16),
              endPoint: V(16f, -16f)
          ),
          color: Color.White,
          resolution: 32
      );

      GeometryBatch.End();
    }
  }
}