using Surreal.Assets;
using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Compute;
using Surreal.Graphics;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Images;
using Surreal.Graphics.Images.FlipBooks;
using Surreal.Graphics.Images.Sprites;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.IO.Json;
using Surreal.IO.Xml;
using Surreal.Mathematics;

namespace Surreal;

/// <summary>Base class for any <see cref="Game"/> that uses rapid prototyping services.</summary>
public abstract class PrototypeGame : Game
{
  public IAudioServer    AudioServer    => Services.GetRequiredService<IAudioServer>();
  public IComputeServer  ComputeServer  => Services.GetRequiredService<IComputeServer>();
  public IGraphicsServer GraphicsServer => Services.GetRequiredService<IGraphicsServer>();
  public IKeyboardDevice Keyboard       => Services.GetRequiredService<IKeyboardDevice>();
  public IMouseDevice    Mouse          => Services.GetRequiredService<IMouseDevice>();

  public Color ClearColor { get; set; } = Color.Black;

  protected override void Initialize()
  {
    base.Initialize();

    OnResized(Host.Width, Host.Height); // initial resize
  }

  protected override void RegisterAssetLoaders(IAssetManager manager)
  {
    base.RegisterAssetLoaders(manager);

    // common
    manager.AddLoader(new JsonAssetLoader());
    manager.AddLoader(new XmlAssetLoader());
    manager.AddLoader(new BlueprintDeclarationLoader());

    // audio
    if (Services.TryGetService(out IAudioServer audioServer))
    {
      manager.AddLoader(new AudioBufferLoader());
      manager.AddLoader(new AudioClipLoader(audioServer));
    }

    // graphics
    if (Services.TryGetService(out IGraphicsServer graphicsServer))
    {
      manager.AddLoader(new BitmapFontLoader(graphicsServer));
      manager.AddLoader(new ColorPaletteLoader());
      manager.AddLoader(new ImageLoader());
      manager.AddLoader(new MaterialLoader());
      manager.AddLoader(new ShaderProgramLoader(graphicsServer, ".shade"));
      manager.AddLoader(new ShaderDeclarationLoader(new ShaderParser(Assets), ".shade"));
      manager.AddLoader(new TextureLoader(graphicsServer, TextureFilterMode.Point, TextureWrapMode.Clamp));
      manager.AddLoader(new TextureRegionLoader());
      manager.AddLoader(new TrueTypeFontLoader());
      manager.AddLoader(new AsepriteImporter
      {
        FilterMode       = TextureFilterMode.Point,
        PixelsPerUnit    = 16,
        FramesPerSecond  = 8,
        TransparencyMask = new Color32(255, 0, 255),
        AnimationFlags   = SpriteAnimationFlags.Looping
      });
    }
  }

  protected override void OnResized(int width, int height)
  {
    base.OnResized(width, height);

    if (Services.TryGetService(out IGraphicsServer graphicsServer))
    {
      graphicsServer.SetViewportSize(new Viewport(0, 0, width, height));
    }
  }

  protected override void BeginFrame(GameTime time)
  {
    if (Services.TryGetService(out IGraphicsServer graphicsServer))
    {
      graphicsServer.ClearColorBuffer(ClearColor);
    }

    base.BeginFrame(time);
  }

  protected override void EndFrame(GameTime time)
  {
    base.EndFrame(time);

    if (Services.TryGetService(out IGraphicsServer graphicsServer))
    {
      graphicsServer.FlushToDevice();
    }
  }
}
