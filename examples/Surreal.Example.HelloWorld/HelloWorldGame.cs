using Surreal.Graphics.Meshes;
using Surreal.Input.Keyboard;

namespace HelloWorld;

public sealed class HelloWorldGame : PrototypeGame
{
  private GraphicsBuffer<float>? vertices;

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

    vertices = new GraphicsBuffer<float>(GraphicsServer);

    vertices.Write(stackalloc float[]
    {
      -0.5f, -0.5f, 0.0f,
      0.5f, -0.5f, 0.0f,
      0.0f, 0.5f, 0.0f,
    });
  }

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape)) Exit();

    base.Input(time);
  }

  protected override void Draw(GameTime time)
  {
    // TODO: draw something
  }

  public override void Dispose()
  {
    vertices?.Dispose();

    base.Dispose();
  }
}
