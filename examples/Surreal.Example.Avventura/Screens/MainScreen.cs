using Surreal.Screens;

namespace Avventura.Screens;

public sealed class MainScreen : IScreen
{
  private readonly IGraphicsServer graphics;
  private readonly IKeyboardDevice keyboard;
  private readonly IMouseDevice mouse;

  public MainScreen(IGraphicsServer graphics, IInputServer input)
  {
    this.graphics = graphics;

    keyboard = input.GetRequiredDevice<IKeyboardDevice>();
    mouse    = input.GetRequiredDevice<IMouseDevice>();
  }

  public void OnUpdate(GameTime time, Game game)
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }
  }

  public void OnRender(GameTime time, Game game)
  {
    graphics.ClearColorBuffer(Color.White);
  }
}
