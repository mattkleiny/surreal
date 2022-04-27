// ReSharper disable AccessToDisposedClosure

using Surreal.Memory;
using Surreal.Pixels;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, async context =>
{
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var input = context.Services.GetRequiredService<IInputServer>();
  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();

  using var shader = await context.Assets.LoadDefaultShaderAsync();
  using var canvas = new PixelCanvas(graphics, 256, 144);

  var position = new Vector2(canvas.Width / 2f, canvas.Height / 2f);

  context.ExecuteVariableStep(time =>
  {
    const int speed = 100;
    const int scale = 32;

    if (keyboard.IsKeyPressed(Key.Escape)) context.Exit();
    if (keyboard.IsKeyDown(Key.W)) position.Y -= speed * time.DeltaTime;
    if (keyboard.IsKeyDown(Key.S)) position.Y += speed * time.DeltaTime;
    if (keyboard.IsKeyDown(Key.A)) position.X -= speed * time.DeltaTime;
    if (keyboard.IsKeyDown(Key.D)) position.X += speed * time.DeltaTime;

    graphics.ClearColorBuffer(Color.Black);

    var pixels = canvas.Span;

    pixels.Fill(Color.White * 0.2f);
    pixels.DrawCircle(position, 32 * scale , Color.Blue);
    pixels.DrawCircle(position, 16 * scale, Color.Green);
    pixels.DrawCircle(position, 8 * scale, Color.Red);
    pixels.DrawCircle(position, 2 * scale, Color.White);

    canvas.Draw(shader);
  });
});
