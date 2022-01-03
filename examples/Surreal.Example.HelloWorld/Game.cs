using Surreal.Graphics.Cameras;
using Surreal.Graphics.Meshes;
using Surreal.Input.Keyboard;
using Surreal.Mathematics;

namespace HelloWorld;

public sealed class Game : PrototypeGame
{
  private readonly OrthographicCamera camera = new(256 / 4, 144 / 4);
  private          GeometryBatch?     batch;

  public static Task Main() => StartAsync<Game>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "Hello, Surreal!",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override void Initialize()
  {
    base.Initialize();

    batch = new GeometryBatch(GraphicsDevice);
  }

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape)) Exit();

    base.Input(time);
  }

  protected override void Update(GameTime time)
  {
    camera.Update();
  }

  protected override void Draw(GameTime time)
  {
    batch?.Begin(null!, in camera.ProjectionView);

    batch?.DrawLine(
      from: -new Vector2(256, 144) / 2f,
      to: new Vector2(256, 144) / 2f,
      color: Color.Red
    );

    batch?.DrawCircle(
      center: new Vector2(-4f, 4f),
      radius: 5f,
      color: Color.Blue,
      segments: 32
    );

    batch?.DrawSolidQuad(
      center: Vector2.Zero,
      size: new Vector2(5f, 10f),
      color: Color.Blue
    );

    batch?.DrawWireQuad(
      center: Vector2.Zero,
      size: new Vector2(5f, 10f) * 2f,
      color: Color.Green
    );

    batch?.DrawArc(
      center: Vector2.Zero,
      startAngle: 0f,
      endAngle: 90f,
      radius: 16f,
      color: new Color(1.0f, 0.5f, 0.25f),
      segments: 32
    );

    batch?.DrawSolidTriangle(
      a: Vector2.Zero,
      b: new Vector2(-16f, 16f),
      c: new Vector2(16f, 16f),
      color: Color.Magenta
    );

    batch?.DrawWireTriangle(
      a: Vector2.Zero,
      b: new Vector2(-16f, -16f),
      c: new Vector2(16f, -16f),
      color: Color.Green
    );

    batch?.DrawCurve(
      curve: new Curves.QuadraticBezier(
        Start: new Vector2(-16f, -16f),
        A: new Vector2(-16, 16),
        End: new Vector2(16f, -16f)
      ),
      color: Color.White,
      resolution: 32
    );
  }

  public override void Dispose()
  {
    batch?.Dispose();

    base.Dispose();
  }
}
