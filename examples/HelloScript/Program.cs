using Surreal.Scripting;
using Surreal.Scripting.Lua;

var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Hello, Scripts!",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080,
      IsTransparent = true
    }
  }
};

Game.Start(configuration, async (Game game, IGraphicsBackend graphics, IKeyboardDevice keyboard) =>
{
  var script = await game.Assets.LoadAssetAsync<Script>("Assets/External/scripts/main.lua");
  var bridge = Variant.From(new ScriptBridge(game, graphics));

  game.ExecuteVariableStep(time =>
  {
    script.ExecuteFunction("tick", bridge, time.DeltaTime.Seconds);

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }
  });
});

/// <summary>
/// An example bridging class for connecting Lua to Surreal.
/// </summary>
[UsedByLua]
public sealed class ScriptBridge(Game game, IGraphicsBackend graphics)
{
  public void Clear(float r, float g, float b, float a)
  {
    graphics.ClearColorBuffer(new Color(r, g, b, a));
  }

  public void Exit()
  {
    game.Exit();
  }
}
