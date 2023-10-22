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
    var graphics = Game.Services.GetServiceOrThrow<IGraphicsBackend>();
    var keyboard = Game.Services.GetServiceOrThrow<IKeyboardDevice>();
    var mouse = Game.Services.GetServiceOrThrow<IMouseDevice>();
    var sprite = await Game.Assets.LoadAssetAsync<Texture>("Assets/External/sprites/bunny.png");

    // setup the render pipeline
    var pipeline = new ForwardRenderPipeline(graphics)
    {
      ClearColor = new Color(0.2f, 0.2f, 0.2f, 0.8f),
      Contexts =
      {
        new SpriteBatchContext(graphics)
      }
    };

    // create scene and main camera
    var scene = new SceneGraph();
    var camera = new CameraNode2D
    {
      Zoom = 100f
    };

    scene.Root.Add(camera);

    // create some bunnies
    for (int i = 0; i < 100; i++)
    {
      scene.Root.Add(new BunnyNode2D
      {
        Sprite = sprite
      });
    }

    return time =>
    {
      scene.Update(time.DeltaTime);
      pipeline.Render(scene);

      camera.Zoom += -mouse.ScrollAmount * zoomSpeed;

      if (keyboard.IsKeyPressed(Key.Escape))
      {
        Game.Exit();
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
