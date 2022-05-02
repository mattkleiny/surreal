using Surreal.Assets;
using Surreal.Audio;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.IO;
using Surreal.Scripting;

namespace Surreal;

/// <summary>A structured entry point for simple games.</summary>
public abstract class PrototypeGame<TSelf> : IDisposable
  where TSelf : PrototypeGame<TSelf>, new()
{
  /// <summary>The current instance of the game.</summary>
  public static TSelf Current { get; private set; } = null!;

  /// <summary>Starts the <see cref="TSelf"/>.</summary>
  public static void Start(IPlatform platform, CancellationToken cancellationToken = default)
  {
    using var game = Current = new TSelf();

    Game.Start(platform, game.OnGameSetup, cancellationToken);
  }

  private Game game = null!;

  public IAudioServer    AudioServer    { get; private set; } = null!;
  public IGraphicsServer GraphicsServer { get; private set; } = null!;
  public IInputServer    InputServer    { get; private set; } = null!;
  public IScriptServer   ScriptServer   { get; private set; } = null!;
  public IKeyboardDevice Keyboard       { get; private set; } = null!;
  public IMouseDevice    Mouse          { get; private set; } = null!;

  public void Exit() => game.Exit();

  /// <summary>Prepares the game and it's dependencies.</summary>
  private async Task OnGameSetup(Game game)
  {
    this.game = game;

    OnRegisterFileSystems(FileSystem.Registry);
    OnRegisterServices(game.Services);

    OnInitializing(game.Host, game.Services);
    OnRegisterAssetLoaders(game.Assets);

    await OnLoadContentAsync(game.Assets);

    OnInitialized(game.Host, game.Services);

    game.ExecuteVariableStep(OnGameTick);
  }

  /// <summary>Callback to register file systems in the system.</summary>
  protected virtual void OnRegisterFileSystems(IFileSystemRegistry registry)
  {
  }

  /// <summary>Callback to register services in the system.</summary>
  protected virtual void OnRegisterServices(IServiceRegistry services)
  {
    services.AddSingleton<IScriptServer>(new LuaScriptServer());
  }

  /// <summary>Callback to register asset loaders in the system.</summary>
  protected virtual void OnRegisterAssetLoaders(IAssetManager assets)
  {
    assets.AddLoader(new ColorPaletteLoader());
    assets.AddLoader(new ScriptLoader(ScriptServer, ".lua"));
  }

  /// <summary>The main callback for loading assets.</summary>
  protected virtual ValueTask OnLoadContentAsync(IAssetManager assets)
  {
    return ValueTask.CompletedTask;
  }

  /// <summary>Called when the game is starting to initialize.</summary>
  protected virtual void OnInitializing(IPlatformHost host, IServiceRegistry services)
  {
    AudioServer    = services.GetRequiredService<IAudioServer>();
    GraphicsServer = services.GetRequiredService<IGraphicsServer>();
    InputServer    = services.GetRequiredService<IInputServer>();
    ScriptServer   = services.GetRequiredService<IScriptServer>();

    Keyboard = InputServer.GetRequiredDevice<IKeyboardDevice>();
    Mouse    = InputServer.GetRequiredDevice<IMouseDevice>();
  }

  /// <summary>Called when the game is initialized and ready to run.</summary>
  protected virtual void OnInitialized(IPlatformHost host, IServiceRegistry services)
  {
  }

  private void OnGameTick(GameTime time)
  {
    OnBeginFrame(time);
    OnInput(time);
    OnUpdate(time);
    OnDraw(time);
    OnEndFrame(time);
  }

  protected virtual void OnBeginFrame(GameTime time)
  {
  }

  protected virtual void OnInput(GameTime time)
  {
  }

  protected virtual void OnUpdate(GameTime time)
  {
  }

  protected virtual void OnDraw(GameTime time)
  {
  }

  protected virtual void OnEndFrame(GameTime time)
  {
  }

  public virtual void Dispose()
  {
  }
}
