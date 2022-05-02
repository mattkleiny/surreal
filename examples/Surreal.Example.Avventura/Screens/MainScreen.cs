using Surreal.Screens;

namespace Avventura.Screens;

public sealed class MainScreen : IScreen
{
  private readonly Game game;
  private readonly IGraphicsServer graphics;
  private readonly IKeyboardDevice keyboard;
  private readonly IScreenManager screens;
  private readonly IServiceRegistry services;
  private readonly Color color;

  public MainScreen(Game game, IGraphicsServer graphics, IKeyboardDevice keyboard, IScreenManager screens, IServiceRegistry services)
  {
    this.game     = game;
    this.graphics = graphics;
    this.keyboard = keyboard;
    this.screens  = screens;
    this.services = services;

    color = Random.Shared.NextColor();
  }

  public void OnUpdate(GameTime time)
  {
    if (keyboard.IsKeyPressed(Key.Space))
    {
      screens.PushScreen(services.Create<MainScreen>());
    }

    graphics.ClearColorBuffer(color);
  }
}
