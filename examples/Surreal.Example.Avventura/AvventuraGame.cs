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

  public AvventuraGame(IGraphicsServer graphics, IKeyboardDevice keyboard)
  {
    this.graphics = graphics;
    this.keyboard = keyboard;
  }

  protected override void OnUpdate(GameTime time)
  {
    base.OnUpdate(time);

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      Exit();
    }
  }

  protected override void OnDraw(GameTime time)
  {
    base.OnDraw(time);

    graphics.ClearColorBuffer(Color.White);
  }
}
