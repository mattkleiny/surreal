﻿var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Falling Sand",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, async game =>
{
  // ReSharper disable AccessToDisposedClosure

  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();
  var mouse = game.Services.GetRequiredService<IMouseDevice>();

  using var material = await game.Assets.LoadDefaultSpriteMaterialAsync();
  using var canvas = new Canvas(graphics, 256, 144);
  using var batch = new SpriteBatch(graphics);

  var palette = await game.Assets.LoadKule16Async();

  var random = Random.Shared;

  game.ExecuteVariableStep(time =>
  {
    graphics.ClearColorBuffer(Color.Black);

    var mousePos = mouse.NormalisedPosition;

    var targetX = mousePos.X * canvas.Width - 1;
    var targetY = mousePos.Y * canvas.Height - 1;

    var point = new Point2((int) targetX, (int) targetY);

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    if (keyboard.IsKeyPressed(Key.Space))
    {
      canvas.Clear();
    }

    if (mouse.IsButtonDown(MouseButton.Left))
    {
      var color = palette[random.Next(2, 6)];

      canvas.AddSand(point, radius: 4, color);
    }

    if (mouse.IsButtonDown(MouseButton.Right))
    {
      canvas.DeleteSand(point, radius: 16);
    }

    batch.Begin(material);

    canvas.Update(time.DeltaTime);
    canvas.DrawNormalized(batch);

    batch.Flush();
  });
});
