using Surreal.Scenes.Canvas;

Game.Start(new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Bunnymark",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080
    }
  },
  Host = GameHost.Create(async () =>
  {
    var graphics = Game.Services.GetServiceOrThrow<IGraphicsBackend>();
    var manager = new RenderContextManager(graphics);
    var scene = new SceneGraph();

    for (int i = 0; i < 100; i++)
    {
      scene.Root.Add(new BunnyNode());
    }

    return time =>
    {
      var frame = new RenderFrame
      {
        DeltaTime = time.DeltaTime,
        Manager = manager
      };

      scene.Update(time.DeltaTime);
      scene.Render(in frame);
    };
  })
});

public sealed class BunnyNode : SpriteNode
{
  public float Speed { get; set; } = Random.Shared.NextFloat() * 10f;

  protected override void OnUpdate(DeltaTime deltaTime)
  {
    base.OnUpdate(deltaTime);

    GlobalPosition += new Vector2(1, 1);
  }
}
