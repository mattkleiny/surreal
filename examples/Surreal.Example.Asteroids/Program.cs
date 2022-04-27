// ReSharper disable AccessToDisposedClosure

using Asteroids.Actors;
using Surreal.Actors;
using Surreal.Pixels;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Asteroids",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
  },
};

Game.Start(platform, async context =>
{
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var input = context.Services.GetRequiredService<IInputServer>();
  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();

  using var shader = await context.Assets.LoadDefaultShaderAsync();
  using var canvas = new PixelCanvas(graphics, 150, 80);
  using var scene = new ActorScene();

  var palette = await context.Assets.LoadAsset<ColorPalette>("resx://Asteroids/Resources/palettes/kule-16.pal");

  var random = Random.Shared;
  var center = new Vector2(150 / 2f, 80 / 2f);

  for (int i = 0; i < 16; i++)
  {
    scene.Spawn(new Asteroid(canvas)
    {
      Position = center + random.NextUnitCircle() * 20f,
      Velocity = random.NextUnitCircle() * random.NextFloat(2f, 4f),
      Rotation = random.NextFloat(0f, 4f),
      Color    = palette[random.Next(1, 4)]
    });
  }

  context.ExecuteVariableStep(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    canvas.Span.Fill(Color.White);

    scene.Input(time.DeltaTime);
    scene.Update(time.DeltaTime);
    scene.Draw(time.DeltaTime);

    canvas.Draw(shader);
  });
});
