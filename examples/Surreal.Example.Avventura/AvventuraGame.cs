using Surreal.Assets;

namespace Avventura;

public sealed class AvventuraGame : Game<AvventuraGame>
{
  public static void Main() => Start(new DesktopPlatform
  {
    Configuration =
    {
      Title          = "Avventura",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      IconPath       = "resx://Avventura/Resources/icons/avventura.png"
    }
  });

  public IAudioServer    AudioServer    { get; }
  public AudioManager    AudioManager   { get; }
  public IGraphicsServer GraphicsServer { get; }
  public IKeyboardDevice Keyboard       { get; }
  public IAssetManager   AssetManager   { get; }

  public AvventuraGame(IAudioServer audio, IGraphicsServer graphicsServer, IKeyboardDevice keyboard, IAssetManager assetManager)
  {
    AudioServer    = audio;
    GraphicsServer = graphicsServer;
    Keyboard       = keyboard;
    AssetManager   = assetManager;
    AudioManager   = new AudioManager(audio, assetManager);
  }

  protected override void OnUpdate(GameTime time)
  {
    base.OnUpdate(time);

    if (Keyboard.IsKeyPressed(Key.Escape))
    {
      Exit();
    }

    AudioManager.Update();
  }

  protected override void OnDraw(GameTime time)
  {
    base.OnDraw(time);

    GraphicsServer.ClearColorBuffer(Color.White);
  }
}
