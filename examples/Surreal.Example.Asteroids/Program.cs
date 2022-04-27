using Asteroids.Actors;
using Surreal.Actors;
using Surreal.Pixels;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Asteroids",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, async context =>
{
  // prepare core services
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var input = context.Services.GetRequiredService<IInputServer>();
  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();

  // load some resources
  using var shader = await context.Assets.LoadDefaultShaderAsync();
  using var canvas = new PixelCanvas(graphics, 256, 144);
  using var scene = new ActorScene();

  var palette = await context.Assets.LoadAsset<ColorPalette>("resx://Asteroids/Resources/palettes/space-dust-9.pal");

  var random = Random.Shared;
  var center = new Vector2(canvas.Width / 2f, canvas.Height / 2f);

  var respawn = () =>
  {
    scene.Clear();

    // spawn the player
    var player = scene.Spawn(new Player(canvas, keyboard)
    {
      Position = center,
      Color    = palette[3]
    });

    // spawn a few asteroids
    for (int i = 0; i < 16; i++)
    {
      scene.Spawn(new Asteroid(canvas, player)
      {
        Position = center + random.NextUnitCircle() * 50f,
        Velocity = random.NextUnitCircle() * random.NextFloat(2f, 12f),
        Rotation = random.NextFloat(0f, MathF.PI),
        Spin     = random.NextFloat(0f, 0.5f),
        Color    = palette[random.NextInt(1, 2)]
      });
    }
  };

  respawn();

  context.ExecuteVariableStep(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    if (keyboard.IsKeyPressed(Key.Space))
    {
      respawn();
    }

    canvas.Span.Fill(Color.Black);

    scene.Input(time.DeltaTime);
    scene.Update(time.DeltaTime);
    scene.Draw(time.DeltaTime);

    canvas.Draw(shader);
  });
});
