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
      Height = 1080,
      IsTransparent = true
    }
  },
  Host = GameHost.Create(async () =>
  {
    var graphics = Game.Services.GetServiceOrThrow<IGraphicsBackend>();
    var keyboard = Game.Services.GetServiceOrThrow<IKeyboardDevice>();

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
      graphics.ClearColorBuffer(new Color(0.2f, 0.2f, 0.2f, 0.8f));

      scene.Update(time.DeltaTime);
      pipeline.Render(scene);

      if (keyboard.IsKeyPressed(Key.Escape))
      {
        Game.Exit();
      }
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
