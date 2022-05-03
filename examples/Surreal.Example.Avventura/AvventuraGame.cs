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

  private readonly IGraphicsServer graphics;
  private readonly IKeyboardDevice keyboard;
  private readonly AudioManager audioManager;

  public AvventuraGame(IAudioServer audio, IGraphicsServer graphics, IKeyboardDevice keyboard, IAssetManager asset)
  {
    this.graphics = graphics;
    this.keyboard = keyboard;

    audioManager = new AudioManager(audio, asset);
  }


  protected override void OnUpdate(GameTime time)
  {
    base.OnUpdate(time);

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      Exit();
    }

    if (keyboard.IsKeyPressed(Key.Space))
    {
      audioManager.PlayBite(SoundBite.ActorDamage);
    }

    audioManager.Update();
  }

  protected override void OnDraw(GameTime time)
  {
    base.OnDraw(time);

    graphics.ClearColorBuffer(Color.White);
  }
}
