using Surreal.UI.Immediate;
using Surreal.UI.Immediate.Controls;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, game =>
{
  // ReSharper disable AccessToDisposedClosure
  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();
  var input = game.Services.GetRequiredService<IInputServer>();

  // TODO: more work on this
  using var ui = new ImmediateModeContext(graphics, input, game.Host, game.Assets);

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    graphics.ClearColorBuffer(Color.White);

    if (ui.Button(new Rectangle(20f, 20f, 200f, 80f), "Button 1"))
    {
      Console.WriteLine("Button 1");
    }

    if (ui.Button(new Rectangle(220f, 20f, 220f + 200f, 80f), "Button 2"))
    {
      Console.WriteLine("Button 2");
    }

    ui.Present();
  });

  return Task.CompletedTask;
});
