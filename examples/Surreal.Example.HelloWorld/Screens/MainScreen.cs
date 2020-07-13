using System.Numerics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Graphics;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Meshes;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Curves;

namespace HelloWorld.Screens {
  public sealed class MainScreen : GameScreen<HelloWorldGame> {
    private readonly OrthographicCamera camera = new OrthographicCamera(256 / 4, 144 / 4);
    private          GeometryBatch      batch;

    public MainScreen(HelloWorldGame game)
        : base(game) {
    }

    protected override async Task LoadContentAsync(IAssetResolver assets) {
      await base.LoadContentAsync(assets);

      batch = await GeometryBatch.CreateDefaultAsync(GraphicsDevice);
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();

      base.Input(time);
    }

    public override void Update(GameTime time) {
      camera.Update();
    }

    public override void Draw(GameTime time) {
      batch.Begin(in camera.ProjectionView);

      batch.DrawLine(
          from: -new Vector2(256, 144) / 2f,
          to: new Vector2(256, 144)    / 2f,
          color: Color.Red
      );

      batch.DrawCircle(
          center: new Vector2(-4f, 4f),
          radius: 5f,
          color: Color.Blue,
          segments: 32
      );

      batch.DrawSolidQuad(
          center: Vector2.Zero,
          size: new Vector2(5f, 10f),
          color: Color.Blue
      );

      batch.DrawWireQuad(
          center: Vector2.Zero,
          size: new Vector2(5f, 10f) * 2f,
          color: Color.Green
      );

      batch.DrawArc(
          center: Vector2.Zero,
          startAngle: 0f,
          endAngle: 90f,
          radius: 16f,
          color: Color.Brown,
          segments: 32
      );

      batch.DrawSolidTriangle(
          a: Vector2.Zero,
          b: new Vector2(-16f, 16f),
          c: new Vector2(16f, 16f),
          color: Color.Magenta
      );

      batch.DrawWireTriangle(
          a: Vector2.Zero,
          b: new Vector2(-16f, -16f),
          c: new Vector2(16f, -16f),
          color: Color.Green
      );

      batch.DrawCurve(
          curve: new QuadraticBezierCurve(
              startPoint: new Vector2(-16f, -16f),
              controlPoint: new Vector2(-16, 16),
              endPoint: new Vector2(16f, -16f)
          ),
          color: Color.White,
          resolution: 32
      );

      batch.End();
    }

    public override void Dispose() {
      batch?.Dispose();

      base.Dispose();
    }
  }
}