using Asteroids;
using Asteroids.Actors;

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
  using var canvas = new Canvas(graphics, 256, 144);
  using var scene = new ActorScene();

  var palette = await context.Assets.LoadAssetAsync<ColorPalette>("resx://Asteroids/Resources/palettes/space-dust-9.pal");

  var random = Random.Shared;
  var center = new Vector2(canvas.Width / 2f, canvas.Height / 2f);

  void Respawn()
  {
    canvas.IsGameOver = false;

    scene.Clear();

    // spawn the player
    var player = scene.Spawn(new Player(canvas, keyboard, scene)
    {
      Position        = center,
      Color           = palette[3],
      ProjectileColor = palette[4]
    });

    // spawn a few asteroids
    for (int i = 0; i < 32; i++)
    {
      scene.Spawn(new Asteroid(canvas, player)
      {
        Position = center + random.NextUnitCircle() * 150f,
        Velocity = random.NextUnitCircle() * random.NextFloat(4f, 12f),
        Rotation = random.NextFloat(0f, MathF.PI),
        Spin     = random.NextFloat(0f, 2f),
        Color    = palette[random.NextInt(1, 2)]
      });
    }
  }

  Respawn();

  context.ExecuteVariableStep(time =>
  {
    if (!context.Host.IsFocused) return;

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    if (keyboard.IsKeyPressed(Key.F4))
    {
      Respawn();
    }

    canvas.Update(time.DeltaTime);

    scene.BeginFrame(time.DeltaTime);
    scene.Input(time.DeltaTime);
    scene.Draw(time.DeltaTime);
    scene.Update(time.DeltaTime);
    scene.EndFrame(time.DeltaTime);

    canvas.Draw(shader);
  });
});
