using Surreal.Assets;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Input.Keyboard;

namespace HelloWorld;

public sealed class HelloWorldGame : PrototypeGame
{
  private static readonly Matrix4x4 ProjectionView = Matrix4x4.CreateOrthographic(256f, 144f, 0f, 100f);

  private Material? material;
  private GeometryBatch? batch;

  public static Task Main() => StartAsync<HelloWorldGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title = "Hello, Surreal!",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override async Task LoadContentAsync(IAssetManager assets, CancellationToken cancellationToken = default)
  {
    await base.LoadContentAsync(assets, cancellationToken);

    material = await assets.LoadAssetAsync<Material>("resx://Surreal.Graphics/Resources/shaders/geometry.shade");
  }

  protected override void Initialize()
  {
    base.Initialize();

    batch = new GeometryBatch(GraphicsServer);
  }

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape))
    {
      Exit();
    }

    base.Input(time);
  }

  protected override void Draw(GameTime time)
  {
    batch!.Begin(material!, in ProjectionView);

    batch!.DrawSolidQuad(Vector2.Zero, new Vector2(16f, 16f), Color.White);
    batch!.DrawCircle(Vector2.Zero, 16f, Color.Red);
  }

  public override void Dispose()
  {
    batch?.Dispose();

    base.Dispose();
  }
}
