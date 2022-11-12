﻿using Surreal.Actors;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title = "Asteroids",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, async game =>
{
  // ReSharper disable AccessToDisposedClosure

  // prepare core services
  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  // load some resources
  using var font = await game.Assets.LoadDefaultBitmapFontAsync();
  using var material = await game.Assets.LoadDefaultSpriteMaterialAsync();

  using var canvas = new Canvas(graphics, 256, 144);
  using var scene = new ActorScene();
  using var batch = new SpriteBatch(graphics);

  var palette = await game.Assets.LoadBuiltInPaletteAsync(BuiltInPalette.SpaceDust9);

  var random = Random.Shared;
  var center = new Vector2(canvas.Width / 2f, canvas.Height / 2f);

  // set-up a basic orthographic projection
  var camera = new Camera
  {
    Position = Vector2.Zero,
    Size = new Vector2(256, 144)
  };

  material.Locals.SetProperty(MaterialProperty.ProjectionView, in camera.ProjectionView);

  void Respawn()
  {
    canvas.IsGameOver = false;

    scene.Clear();

    // spawn the player
    var player = scene.Spawn(new Player(canvas, keyboard, scene)
    {
      Position = center,
      Color = palette[3],
      ProjectileColor = palette[4]
    });

    // spawn a few asteroids
    for (var i = 0; i < 32; i++)
      scene.Spawn(new Asteroid(canvas, player)
      {
        Position = center + random.NextUnitCircle() * 150f,
        Velocity = random.NextUnitCircle() * random.NextFloat(4f, 12f),
        Rotation = random.NextFloat(0f, MathF.PI),
        Spin = random.NextFloat(0f, 2f),
        Color = palette[random.NextInt(1, 2)]
      });
  }

  Respawn();

  game.ExecuteVariableStep(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    if (keyboard.IsKeyPressed(Key.R))
    {
      Respawn();
    }

    batch.Begin(material);

    canvas.Update(time.DeltaTime);
    scene.Tick(time.DeltaTime);
    canvas.Draw(batch);

    if (canvas.IsGameOver)
    {
      batch.DrawText(
        font,
        "GAME OVER!",
        Vector2.Zero,
        Vector2.One * 1.4f,
        Color.White,
        horizontalAlignment: HorizontalAlignment.Center,
        verticalAlignment: VerticalAlignment.Center
      );
    }

    batch.Flush();
  });
});
