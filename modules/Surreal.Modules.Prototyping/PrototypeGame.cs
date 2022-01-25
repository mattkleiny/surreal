using Surreal.Assets;
using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Compute;
using Surreal.Graphics;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Fonts;
using Surreal.Graphics.Images;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.IO.Json;
using Surreal.IO.Xml;
using Surreal.Mathematics;
using Surreal.Networking;
using Surreal.Scripting;
using Surreal.Scripting.Bytecode;
using Surreal.Scripting.Languages;

namespace Surreal;

/// <summary>Base class for any <see cref="Game"/> that uses rapid prototyping services.</summary>
public abstract class PrototypeGame : Game
{
  public IAudioServer    AudioServer    => Services.GetRequiredService<IAudioServer>();
  public IComputeServer  ComputeServer  => Services.GetRequiredService<IComputeServer>();
  public IGraphicsServer GraphicsServer => Services.GetRequiredService<IGraphicsServer>();
  public IKeyboardDevice Keyboard       => Services.GetRequiredService<IKeyboardDevice>();
  public IMouseDevice    Mouse          => Services.GetRequiredService<IMouseDevice>();
  public INetworkFactory NetworkFactory => Services.GetRequiredService<INetworkFactory>();

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

    // audio
    manager.AddLoader(new AudioBufferLoader());
    manager.AddLoader(new AudioClipLoader(AudioServer));

    // graphics
    manager.AddLoader(new BitmapFontLoader());
    manager.AddLoader(new ColorPaletteLoader());
    manager.AddLoader(new ImageLoader());
    manager.AddLoader(new ShaderProgramLoader(GraphicsServer, ".shade"));
    manager.AddLoader(new ShaderDeclarationLoader(new StandardShaderParser(Assets), ".shade"));
    manager.AddLoader(new TextureLoader(GraphicsServer, TextureFilterMode.Point, TextureWrapMode.Clamp));
    manager.AddLoader(new TextureRegionLoader());
    manager.AddLoader(new TrueTypeFontLoader());

    // scripting
    manager.AddLoader(new BytecodeProgramLoader());
    manager.AddLoader(new ScriptLoader(new BytecodeScriptCompiler(), ".basic", ".bas", ".lisp", ".lox", ".lua", ".wren"));
    manager.AddLoader(new ScriptDeclarationLoader(new BasicScriptParser(), ".basic", ".bas"));
    manager.AddLoader(new ScriptDeclarationLoader(new LispScriptParser(), ".lisp"));
    manager.AddLoader(new ScriptDeclarationLoader(new LoxScriptParser(), ".lox"));
    manager.AddLoader(new ScriptDeclarationLoader(new LuaScriptParser(), ".lua"));
    manager.AddLoader(new ScriptDeclarationLoader(new WrenScriptParser(), ".wren"));
  }

  protected override void OnResized(int width, int height)
  {
    base.OnResized(width, height);

    GraphicsServer.SetViewportSize(new Viewport(0, 0, width, height));
  }

  protected override void BeginFrame(GameTime time)
  {
    GraphicsServer.ClearColorBuffer(ClearColor);

    base.BeginFrame(time);
  }

  protected override void EndFrame(GameTime time)
  {
    base.EndFrame(time);

    GraphicsServer.FlushToDevice();
  }
}
