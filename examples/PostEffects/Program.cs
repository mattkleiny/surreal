using Surreal.Graphics.Rendering.Effects;

var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Post Effects",
      Width = 1280,
      Height = 1024,
      IsVsyncEnabled = true
    }
  }
};

Game.Start(configuration, (Game game, IGraphicsDevice graphics) =>
{
  var pipeline = new ForwardRenderPipeline(graphics)
  {
    Effects =
    {
      new ChromaticAberrationEffect(graphics)
    }
  };

  game.Render += _ =>
  {
    pipeline.Render(IRenderScene.Null, DeltaTime.Default);
  };

  game.ExecuteVariableStep();
});
