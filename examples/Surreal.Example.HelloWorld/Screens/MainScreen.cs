using System.Numerics;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Graphics;
using Surreal.Graphics.Cameras;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Curves;

namespace HelloWorld.Screens {
  public sealed class MainScreen : GameScreen<Game> {
    private readonly OrthographicCamera camera = new OrthographicCamera(256 / 4, 144 / 4);

    public MainScreen(Game game)
        : base(game) {
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();

      base.Input(time);
    }

    public override void Draw(GameTime time) {
      GeometryBatch.Begin(in camera.ProjectionView);

      GeometryBatch.DrawLine(
          from: -new Vector2(256, 144) / 2f,
          to: new Vector2(256, 144) / 2f,
          color: Color.Red
      );

      GeometryBatch.DrawCircle(
          center: new Vector2(-4f, 4f),
          radius: 5f,
          color: Color.Blue,
          segments: 32
      );

      GeometryBatch.DrawSolidQuad(
          center: Vector2.Zero,
          size: new Vector2(5f, 10f),
          color: Color.Blue
      );

      GeometryBatch.DrawWireQuad(
          center: Vector2.Zero,
          size: new Vector2(5f, 10f) * 2f,
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
          b: new Vector2(-16f, 16f),
          c: new Vector2(16f, 16f),
          color: Color.Magenta
      );

      GeometryBatch.DrawWireTriangle(
          a: Vector2.Zero,
          b: new Vector2(-16f, -16f),
          c: new Vector2(16f, -16f),
          color: Color.Green
      );

      GeometryBatch.DrawCurve(
          curve: new QuadraticBezierCurve(
              startPoint: new Vector2(-16f, -16f),
              controlPoint: new Vector2(-16, 16),
              endPoint: new Vector2(16f, -16f)
          ),
          color: Color.White,
          resolution: 32
      );

      GeometryBatch.End();
    }
  }
}