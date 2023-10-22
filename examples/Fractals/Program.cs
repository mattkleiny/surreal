const int maxIterations = 35;

Game.Start(new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Fractals",
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

    var canvas = new PixelCanvas(graphics, 256, 144);

    var palette = await Game.Assets.LoadAssetAsync<ColorPalette>("resx://Fractals/Assets/Embedded/palettes/raspberry.pal");
    var constant = new Vector2(0.285f, 0.01f);

    return time =>
    {
      constant += new Vector2(0.001f, 0.0f) * time.DeltaTime;
      graphics.ClearColorBuffer(new Color(0.2f, 0.2f, 0.2f, 0.8f));

      DrawFractal(palette, canvas, constant);

      canvas.DrawQuad();

      if (keyboard.IsKeyPressed(Key.Escape))
      {
        Game.Exit();
      }
    };

    static void DrawFractal(ColorPalette palette, PixelCanvas canvas, Vector2 constant)
    {
      var pixels = canvas.Pixels;
      var scale = 1f / (pixels.Height / 2f);

      for (var y = 0; y < pixels.Height; y++)
      for (var x = 0; x < pixels.Width; x++)
      {
        var initial = new Vector2(
          (x - pixels.Width / 2f) * scale,
          (y - pixels.Height / 2f) * scale
        );

        var iterations = ComputeIterations(initial, constant);

        pixels[x, y] = (Color)palette[iterations % palette.Count] - new Color(0, 0, 0, iterations / (float)maxIterations);
      }
    }

    static Vector2 ComputeNext(Vector2 initial, Vector2 constant)
    {
      var zr = initial.X * initial.X - initial.Y * initial.Y + constant.X;
      var zi = 2f * initial.X * initial.Y + constant.Y;

      return new Vector2(zr, zi);
    }

    static int ComputeIterations(Vector2 initial, Vector2 constant)
    {
      var current = initial;
      var iterations = 0;

      while (current.LengthSquared() < 4 && iterations < maxIterations)
      {
        current = ComputeNext(current, constant);
        iterations++;
      }

      return iterations;
    }
  })
});
