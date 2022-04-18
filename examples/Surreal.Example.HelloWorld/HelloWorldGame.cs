using Surreal.Assets;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Input.Keyboard;

namespace HelloWorld;

public sealed class HelloWorldGame : PrototypeGame
{
  private static readonly Matrix4x4 ProjectionView = Matrix4x4.CreateOrthographic(256f, 144f, 0f, 100f);

  private Material? geometryMaterial;
  private GeometryBatch? geometryBatch;

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

    geometryMaterial = await assets.LoadAssetAsync<Material>("resx://Surreal.Graphics/Resources/shaders/geometry.shade");
  }

  protected override void Initialize()
  {
    base.Initialize();

    geometryBatch = new GeometryBatch(GraphicsServer);
  }

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape))
    {
      Exit();
    }

    base.Input(time);
  }

  protected override void BeginFrame(GameTime time)
  {
    base.BeginFrame(time);

    geometryBatch!.Begin(geometryMaterial!, in ProjectionView);
  }

  protected override void Draw(GameTime time)
  {
    geometryBatch!.DrawSolidQuad(Vector2.Zero, new Vector2(16f, 16f), Color.White);
    geometryBatch!.DrawCircle(Vector2.Zero, 16f, Color.Red);
  }

  public override void Dispose()
  {
    geometryBatch?.Dispose();

    base.Dispose();
  }
}
