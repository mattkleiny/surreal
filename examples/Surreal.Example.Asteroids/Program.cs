using Asteroids.Actors;
using Surreal.Actors;

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
  using var canvas = new AsteroidsCanvas(graphics, 256, 144);
  using var scene = new ActorScene();

  var palette = await context.Assets.LoadAsset<ColorPalette>("resx://Asteroids/Resources/palettes/space-dust-9.pal");

  var random = Random.Shared;
  var center = new Vector2(canvas.Width / 2f, canvas.Height / 2f);

  void Respawn()
  {
    canvas.IsExploding = false;

    scene.Clear();

    // spawn the player
    var player = scene.Spawn(new Player(canvas, keyboard, scene)
    {
      Position = center,
      Color    = palette[3]
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
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    if (keyboard.IsKeyPressed(Key.Space))
    {
      Respawn();
    }

    canvas.Update(time.DeltaTime);

    scene.Input(time.DeltaTime);
    scene.Draw(time.DeltaTime);
    scene.Update(time.DeltaTime);

    canvas.Draw(shader);
  });
});
