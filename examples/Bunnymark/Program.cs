const int width = 1920;
const int height = 1080;
const int batchSize = 512;

var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Bunnymark",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = width,
      Height = height,
      IsTransparent = true
    }
  },
};

Game.Start(configuration, async game =>
{
  var log = LogFactory.GetLog<Program>();

  var graphics = game.Services.GetServiceOrThrow<IGraphicsBackend>();
  var keyboard = game.Services.GetServiceOrThrow<IKeyboardDevice>();
  var mouse = game.Services.GetServiceOrThrow<IMouseDevice>();

  var sprite = await game.Assets.LoadAssetAsync<Texture>("Assets/External/sprites/bunny.png");

  var batch = new SpriteBatch(graphics);
  var material = new Material(graphics, ShaderProgram.LoadDefaultSpriteShader(graphics))
  {
    BlendState = BlendState.OneMinusSourceAlpha,
    Properties =
    {
      { "u_texture", sprite },
      { "u_transform", Matrix4x4.CreateOrthographic(width, -height, 0, 100f) }
    }
  };

  var bunnies = new List<Bunny>();

  var size = new Vector2(sprite.Width, sprite.Height);

  game.ExecuteVariableStep(time =>
  {
    graphics.ClearColorBuffer(new Color(0.2f, 0.2f, 0.2f, 0.8f));

    foreach (ref var bunny in bunnies.AsSpan())
    {
      // update position and rotation
      bunny.Position += bunny.Velocity * 100f * time.DeltaTime;
      bunny.Rotation += Angle.FromRadians(bunny.RotationSpeed * time.DeltaTime);

      // bounce off walls
      if (bunny.Position.X < -width / 2f) bunny.Velocity.X *= -1f;
      if (bunny.Position.X > width / 2f) bunny.Velocity.X *= -1f;
      if (bunny.Position.Y < -height / 2f) bunny.Velocity.Y *= -1f;
      if (bunny.Position.Y > height / 2f) bunny.Velocity.Y *= -1f;
    }

    batch.Begin(material);

    foreach (ref var bunny in bunnies.AsSpan())
    {
      batch.Draw(sprite, bunny.Position, size, bunny.Rotation.Radians, bunny.Tint);
    }

    batch.Flush();

    if (mouse.IsButtonDown(MouseButton.Left))
    {
      var relativePosition = new Vector2(
        mouse.NormalisedPosition.X * width - width / 2f,
        mouse.NormalisedPosition.Y * height - height / 2f
      );

      for (int i = 0; i < batchSize; i++)
      {
        bunnies.Add(new Bunny
        {
          Position = relativePosition,
          Velocity = Random.Shared.NextVector2(-1f, 1f),
          Tint = Random.Shared.Next<Color32>(),
          Rotation = Angle.Zero,
          RotationSpeed = Random.Shared.NextFloat(-1f, 1f)
        });

        log.Trace($"There are {bunnies.Count} bunnies");
      }
    }
    else if (mouse.IsButtonDown(MouseButton.Right))
    {
      for (int i = 0; i < batchSize; i++)
      {
        if (bunnies.Count > 0)
        {
          bunnies.RemoveAt(bunnies.Count - 1);
        }
      }

      log.Trace($"There are {bunnies.Count} bunnies");
    }

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }
  });
});

/// <summary>
/// A bunny in the sprite benchmark.
/// </summary>
public struct Bunny
{
  public required Vector2 Position;
  public required Vector2 Velocity;
  public required Color32 Tint;
  public required Angle Rotation;
  public required float RotationSpeed;
}
