using Sand;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Falling Sand",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
  },
};

Game.Start(platform, async context =>
{
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var input = context.Services.GetRequiredService<IInputServer>();
  var mouse = input.GetRequiredDevice<IMouseDevice>();

  using var shader = await context.Assets.LoadDefaultShaderAsync();
  using var canvas = new SandCanvas(graphics, 256, 144);

  var palette = await context.Assets.LoadAsset<ColorPalette>("resx://Sand/Resources/palettes/kule-16.pal");
  var random = Random.Shared;

  context.ExecuteVariableStep(time =>
  {
    graphics.ClearColorBuffer(Color.Black);

    var normalizedX = mouse.Position.X / context.Host.Width;
    var normalizedY = mouse.Position.Y / context.Host.Height;

    var targetX = normalizedX * canvas.Width - 1;
    var targetY = normalizedY * canvas.Height - 1;

    var point = new Point2((int) targetX, (int) targetY);

    if (mouse.IsButtonDown(MouseButton.Left))
    {
      var color = palette[random.Next(2, 4)];

      canvas.AddSand(point, radius: 16, color);
    }

    if (mouse.IsButtonDown(MouseButton.Right))
    {
      canvas.DeleteSand(point, radius: 16);
    }

    canvas.Update(time.DeltaTime);
    canvas.Draw(shader);
  });
});
