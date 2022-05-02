using RayTracer.Scenes;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Ray Tracer",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, async game =>
{
  // ReSharper disable AccessToDisposedClosure
  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  using var shader = await game.Assets.LoadDefaultSpriteShaderAsync();
  using var buffer = new PixelCanvas(graphics, 1920, 1080);

  // build a ray-tracing scene
  var scene = new Scene
  {
    FieldOfView = 90f,
    Background  = Color32.White,
    Width       = buffer.Width,
    Height      = buffer.Height,
    Nodes =
    {
      new Sphere(new(5f, -1f, -15f), 2f, new Material.Colored(Color32.Blue))
    }
  };

  void RefreshScene()
  {
    // start up some parallel work to process the image
    const int chunkCount = 32;

    var unitsX = buffer.Width / chunkCount;
    var unitsY = buffer.Height / chunkCount;

    for (int y = 0; y < unitsY; y++)
    for (int x = 0; x < unitsX; x++)
    {
      var copyX = x;
      var copyY = y;

      Task.Run(() =>
      {
        var startX = copyX * unitsX;
        var endX = copyX * unitsX + unitsX;

        var startY = copyY * unitsY;
        var endY = copyY * unitsY + unitsY;

        var pixels = buffer.Pixels;

        for (int y = startY; y < endY; y++)
        for (int x = startX; x < endX; x++)
        {
          if (x < pixels.Width && y < pixels.Height)
          {
            pixels[x, y] = scene.CastRay(new(x, y));
          }
        }
      });
    }
  }

  RefreshScene();

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    if (keyboard.IsKeyPressed(Key.Space))
    {
      RefreshScene();
    }

    buffer.Draw(shader);
  });
});
