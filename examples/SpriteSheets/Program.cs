var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Sprite Sheets",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1024,
      Height = 768,
    },
  },
};

return Game.Start(configuration, (Game game, IGraphicsDevice graphics) =>
{
  var scene = new SceneTree
  {
    Assets = game.Assets,
    Services = game.Services,
    Renderer = new ForwardRenderPipeline(graphics)
    {
      ClearColor = Color.Black
    }
  };

  game.ExecuteScene(scene);
});
