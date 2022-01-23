using Surreal.Graphics.Cameras;
using Surreal.Graphics.Meshes;
using Surreal.Input.Keyboard;

namespace HelloWorld;

public sealed class HelloWorldGame : PrototypeGame
{
  private readonly OrthographicCamera camera = new(256 / 4, 144 / 4);
  private          GeometryBatch?     batch;

  public static Task Main() => StartAsync<HelloWorldGame>(new Configuration
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

    batch = new GeometryBatch(GraphicsServer);
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
    // TODO: draw something
  }

  public override void Dispose()
  {
    batch?.Dispose();

    base.Dispose();
  }
}
