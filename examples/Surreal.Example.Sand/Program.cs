﻿using Sand;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Falling Sand",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, async context =>
{
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var input = context.Services.GetRequiredService<IInputServer>();
  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();
  var mouse = input.GetRequiredDevice<IMouseDevice>();

  using var shader = await context.Assets.LoadDefaultShaderAsync();
  using var canvas = new SandCanvas(graphics, 256, 144);

  var palette = await context.Assets.LoadAssetAsync<ColorPalette>("resx://Sand/Resources/palettes/kule-16.pal");
  var random = Random.Shared;

  context.ExecuteVariableStep(time =>
  {
    if (!context.Host.IsFocused) return;

    graphics.ClearColorBuffer(Color.Black);

    var mousePos = mouse.NormalisedPosition;

    var targetX = mousePos.X * canvas.Width - 1;
    var targetY = mousePos.Y * canvas.Height - 1;

    var point = new Point2((int)targetX, (int)targetY);

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
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

    canvas.Update(time.DeltaTime);
    canvas.Draw(shader);
  });
});