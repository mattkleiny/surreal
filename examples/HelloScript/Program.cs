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
  using var geometryBatch = new GeometryBatch(graphics)
  {
    Material = new Material(graphics, ShaderProgram.LoadDefaultWireShader(graphics))
  };

  var script = await game.Assets.LoadAssetAsync<Script>("Assets/External/scripts/main.lua");
  var bridge = Variant.From(new ScriptBridge(game, graphics, geometryBatch));

  game.ExecuteVariableStep(time =>
  {
    script.ExecuteFunction("tick", bridge, time.DeltaTime.Seconds);

    geometryBatch.Flush();

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
public sealed class ScriptBridge(Game game, IGraphicsBackend graphics, IGizmoBatch gizmos)
{
  public void DrawLine(Vector2 a, Vector2 b)
  {
    DrawLine(a, b, Color.White);
  }

  public void DrawLine(Vector2 a, Vector2 b, Color color)
  {
    gizmos.DrawLine(a, b, color);
  }

  public void Clear(float r, float g, float b, float a)
  {
    graphics.ClearColorBuffer(new Color(r, g, b, a));
  }

  public void Exit()
  {
    game.Exit();
  }
}
