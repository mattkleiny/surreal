using Surreal.Graphics.Sprites;

var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Sprite Sheest",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080,
      IsTransparent = true
    }
  },
  Modules =
  {
    // we'll use aseprite files directly in this example
    new AsepriteModule()
  }
};

Game.Start(configuration, async (Game game, IGraphicsBackend graphics, IKeyboardDevice keyboard) =>
{
  var spriteSheet = await game.Assets.LoadAssetAsync<SpriteSheet>("Assets/External/sprites/crab.aseprite");

  game.ExecuteVariableStep(_ =>
  {
    graphics.ClearColorBuffer(new Color(0.2f, 0.2f, 0.2f, 0.8f));

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }
  });
});
