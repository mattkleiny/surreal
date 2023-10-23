const float zoomSpeed = 10f;

var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Scene Graph Example",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080,
      IsTransparent = true
    }
  }
};

Game.Start(configuration, async game =>
{
  var graphics = game.Services.GetServiceOrThrow<IGraphicsBackend>();
  var keyboard = game.Services.GetServiceOrThrow<IKeyboardDevice>();
  var mouse = game.Services.GetServiceOrThrow<IMouseDevice>();
  var debug = game.Services.GetServiceOrThrow<IDebugGui>();

  var clip = await game.Assets.LoadAssetAsync<AudioClip>("Assets/External/audio/test.wav");
  var sprite = await game.Assets.LoadAssetAsync<Texture>("Assets/External/sprites/bunny.png");

  // setup the render pipeline
  using var pipeline = new ForwardRenderPipeline(graphics)
  {
    ClearColor = new Color(0.2f, 0.2f, 0.2f, 0.8f),
    Contexts =
    {
      new SpriteContext(graphics)
    }
  };

  // create scene and main camera
  using var scene = new SceneGraph
  {
    Assets = game.Assets,
    Services = game.Services
  };

  var viewport = new CameraViewportNode();

  scene.Add(viewport);

  var camera = new CameraNode2D
  {
    Zoom = 100f
  };

  // add the music player
  viewport.Add(new AudioPlayer2D
  {
    PlayOnReady = true,
    IsLooping = true,
    AudioClip = clip
  });

  viewport.Add(camera);

  // create some bunnies
  for (int i = 0; i < 100; i++)
  {
    viewport.Add(new Bunny
    {
      Sprite = sprite
    });
  }

  game.ExecuteVariableStep(time =>
  {
    scene.Update(time.DeltaTime);
    pipeline.Render(scene);

    camera.Zoom += -mouse.ScrollAmount * zoomSpeed;

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    debug.ShowWindow("Debug Tools", window =>
    {
      var visibleObjects = viewport.CullVisibleObjects<IRenderObject>();

      window.Text($"Delta Time: {time.DeltaTime}");
      window.Text($"Total Time: {time.TotalTime}");
      window.Text($"Frames per second: {time.FramesPerSecond}");
      window.Text($"Visible objects: {visibleObjects.Length}");

      pipeline.EnableGizmos = window.Checkbox("Enable Gizmos", pipeline.EnableGizmos);
    });
  });
});


/// <summary>
/// An example custom node.
/// </summary>
internal sealed class Bunny : SpriteNode2D
{
  /// <summary>
  /// The speed of the bunny.
  /// </summary>
  public float Speed { get; set; } = 10f;

  /// <summary>
  /// The velocity of the bunny.
  /// </summary>
  public Vector2 Velocity { get; set; } = Vector2.Zero;

  protected override void OnUpdate(DeltaTime deltaTime)
  {
    base.OnUpdate(deltaTime);

    Velocity += new Vector2(
      (Random.Shared.NextFloat() - 0.5f) * Speed,
      (Random.Shared.NextFloat() - 0.5f) * Speed
    );

    GlobalPosition += Velocity * deltaTime;
  }
}
