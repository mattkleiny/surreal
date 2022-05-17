using Material = Surreal.Material;
using Plane = Surreal.Plane;

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

  using var buffer = new PixelCanvas(graphics, 1920, 1080);
  using var material = await game.Assets.LoadDefaultSpriteMaterialAsync();

  var checkerboard = await game.Assets.LoadAssetAsync<Image>("Assets/textures/checkerboard.png");

  // build a ray-tracing scene
  var scene = new Scene
  {
    Width  = buffer.Width,
    Height = buffer.Height,
    Nodes =
    {
      new Sphere(new(5f, -1f, -15f), 2f, new Material.Solid(Color.Blue, 0.6f)),
      new Sphere(new(3f, 0f, -35f), 1f, new Material.Solid(Color.Green, 0.3f)),
      new Sphere(new(3f, 2f, -20f), 1f, new Material.Solid(Color.Red, 0.3f)),
      new Sphere(new(-5.5f, 0f, -15f), 1f, new Material.Textured(checkerboard, 0.2f)),
      new Plane(new(0f, -4.2f, 0f), new(0f, -1f, 0f), new Material.Solid(Color.White, 0.1f)),
    },
    Lights =
    {
      new Light(new(-1f, -1f, 0f), Color.White, 1.0f),
      new Light(new(1f, -1f, 0f), Color.White, 1.0f),
    }
  };

  void RefreshSequentially()
  {
    Task.Run(() =>
    {
      buffer.Pixels.Fill(scene.Background);

      var pixels = buffer.Pixels;

      for (int y = 0; y < buffer.Height; y++)
      for (int x = 0; x < buffer.Width; x++)
      {
        if (x < pixels.Width && y < pixels.Height)
        {
          pixels[x, y] = scene.Sample(new(x, y));
        }
      }
    });
  }

  void RefreshInParallel()
  {
    // start up some parallel work to process the image
    const int chunkCount = 32;

    buffer.Pixels.Fill(scene.Background);

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
            pixels[x, y] = scene.Sample(new(x, y));
          }
        }
      });
    }
  }

  RefreshInParallel();

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape)) game.Exit();
    if (keyboard.IsKeyPressed(Key.F1)) RefreshSequentially();
    if (keyboard.IsKeyPressed(Key.F2)) RefreshInParallel();

    buffer.Draw(material);
  });
});
