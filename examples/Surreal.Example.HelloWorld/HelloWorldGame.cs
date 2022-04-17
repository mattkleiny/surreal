using Surreal.Assets;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Input.Keyboard;

namespace HelloWorld;

public sealed class HelloWorldGame : PrototypeGame
{
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

    geometryMaterial = await assets.LoadAssetAsync<Material>("resx://HelloWorld/Resources/shaders/geometry.shade");
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

    var projectionView = Matrix4x4.CreateOrthographic(256f, 144f, 0f, 100f);

    geometryBatch!.Begin(geometryMaterial!, projectionView);
  }

  protected override void Draw(GameTime time)
  {
    // TODO: draw something
  }

  public override void Dispose()
  {
    geometryBatch?.Dispose();

    base.Dispose();
  }
}
