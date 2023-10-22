const float zoomSpeed = 10f;

var configuration = new GameConfiguration
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
  }
};

Game.Start(configuration, async game =>
{
  var audio = game.Services.GetServiceOrThrow<IAudioBackend>();
  var graphics = game.Services.GetServiceOrThrow<IGraphicsBackend>();
  var keyboard = game.Services.GetServiceOrThrow<IKeyboardDevice>();
  var mouse = game.Services.GetServiceOrThrow<IMouseDevice>();

  var clip = await game.Assets.LoadAssetAsync<AudioClip>("Assets/External/audio/test.wav");
  var sprite = await game.Assets.LoadAssetAsync<Texture>("Assets/External/sprites/bunny.png");

  var source = new AudioSource(audio);

  // setup the render pipeline
  using var pipeline = new ForwardRenderPipeline(graphics)
  {
    ClearColor = new Color(0.2f, 0.2f, 0.2f, 0.8f),
    Contexts =
    {
      new SpriteContext(graphics),
      new WidgetContext(graphics)
    }
  };

  // create scene and main camera
  using var scene = new SceneGraph();

  var viewport = new CameraViewportNode();
  var camera = new CameraNode2D
  {
    Zoom = 100f
  };

  scene.Root.Add(viewport);
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

    if (keyboard.IsKeyPressed(Key.Space))
    {
      source.Play(clip);
    }
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
