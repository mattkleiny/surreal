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
        new SpriteContext(graphics),
        new WidgetContext(graphics)
      }
    };

    // create scene and main camera
    var scene = new SceneGraph();

    var viewport = new CameraViewportNode();
    var widgets = new WidgetViewportNode
    {
      Widget = new FloatingWindow
      {
        new StackPanel
        {
          new TextBlock
          {
            Text = "Bunnymark",
            FontSize = 24f,
            Margin = 8f
          },
          new TextBlock
          {
            Text = "Press ESC to exit",
            FontSize = 16f,
            Margin = 8f
          }
        }
      }
    };

    var camera = new CameraNode2D
    {
      Zoom = 100f
    };

    scene.Root.Add(viewport);
    scene.Root.Add(widgets);

    viewport.Add(camera);

    // create some bunnies
    for (int i = 0; i < 100; i++)
    {
      viewport.Add(new Bunny
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
