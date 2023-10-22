const float zoomSpeed = 10f;

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
    var log = LogFactory.GetLog<Program>();
    var logTimer = new IntervalTimer(TimeSpan.FromSeconds(1));

    var graphics = Game.Services.GetServiceOrThrow<IGraphicsBackend>();
    var keyboard = Game.Services.GetServiceOrThrow<IKeyboardDevice>();
    var mouse = Game.Services.GetServiceOrThrow<IMouseDevice>();

    var pipeline = new ForwardRenderPipeline(graphics)
    {
      Contexts =
      {
        new SpriteBatchContext(graphics)
      }
    };

    var sprite = await Game.Assets.LoadAssetAsync<Texture>("Assets/External/sprites/bunny.png");
    var scene = new SceneGraph();

    var camera = new CameraNode2D
    {
      Zoom = 100f
    };

    scene.Root.Add(camera);

    for (int i = 0; i < 100; i++)
    {
      scene.Root.Add(new BunnyNode2D
      {
        Sprite = sprite
      });
    }

    return time =>
    {
      graphics.ClearColorBuffer(new Color(0.2f, 0.2f, 0.2f, 0.8f));

      scene.Update(time.DeltaTime);
      pipeline.Render(scene);

      camera.Zoom += -mouse.ScrollAmount * zoomSpeed;

      if (keyboard.IsKeyPressed(Key.Escape))
      {
        Game.Exit();
      }

      if (logTimer.Tick(time.DeltaTime))
      {
        var visibleObjects = camera.CullVisibleObjects();

        log.Trace($"There are {visibleObjects.Length} visible objects");

        logTimer.Reset();
      }
    };
  })
});

internal sealed class BunnyNode2D : SpriteNode2D
{
  public float Speed { get; set; } = 10f;
  public Vector2 Velocity { get; set; } = Vector2.Zero;

  protected override void OnUpdate(DeltaTime deltaTime)
  {
    base.OnUpdate(deltaTime);

    Velocity += new Vector2(
      (float)(Random.Shared.NextDouble() - 0.5f) * Speed,
      (float)(Random.Shared.NextDouble() - 0.5f) * Speed
    );

    GlobalPosition += Velocity * deltaTime;
  }
}
