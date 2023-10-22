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
    var pipeline = new ForwardRenderPipeline(graphics)
    {
      Contexts =
      {
        new SpriteBatchContext(graphics)
      }
    };

    var sprite = await Game.Assets.LoadAssetAsync<Texture>("Assets/External/sprites/bunny.png");
    var scene = new SceneGraph();

    scene.Root.Add(new CameraNode2D());

    for (int i = 0; i < 10; i++)
    {
      scene.Root.Add(new BunnyNode
      {
        Sprite = sprite
      });
    }

    return time =>
    {
      scene.Update(time.DeltaTime);
      pipeline.Render(scene);
    };
  })
});

public sealed class BunnyNode : SpriteNode
{
  public float Speed { get; set; } = Random.Shared.NextFloat() * 10f;

  protected override void OnUpdate(DeltaTime deltaTime)
  {
    base.OnUpdate(deltaTime);

    GlobalPosition += new Vector2(1, 1) * Speed * deltaTime.Seconds;
  }
}
