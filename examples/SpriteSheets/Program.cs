var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Sprite Sheets",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080,
      IsTransparent = true
    }
  }
};

return Game.Start(configuration, async (Game game, IGraphicsDevice graphics, IKeyboardDevice keyboard) =>
{
  var sprites = await game.Assets.LoadAsync<SpriteAnimationSet>("Assets/External/sprites/crab.aseprite");

  game.ExecuteVariableStep(_ =>
  {
    graphics.ClearColorBuffer(new Color(0.2f, 0.2f, 0.2f, 0.8f));

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }
  });
});
