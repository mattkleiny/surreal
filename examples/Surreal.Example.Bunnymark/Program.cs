var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Bunnymark",
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

  using var sprite = await game.Assets.LoadAssetAsync<Texture>("Assets/wabbit_alpha.png");
  using var font = await game.Assets.LoadDefaultBitmapFontAsync();

  using var spriteMaterial = await game.Assets.LoadDefaultSpriteMaterialAsync();
  using var aberrationMaterial = await game.Assets.LoadAberrationMaterialAsync();

  using var batch = new SpriteBatch(graphics, spriteCount: 8000);
  using var target = new RenderTarget(graphics, RenderTargetDescriptor.Default with { Width = 256, Height = 144 });

  var camera = new Camera
  {
    Position = new Vector2(0f, 0f),
    Size     = new Vector2(1920f, 1080f)
  };

  var actors = new List<Bunny>();

  spriteMaterial.Properties.Set(Material.DefaultProjectionView, in camera.ProjectionView);
  aberrationMaterial.Properties.Set(Material.DefaultProjectionView, in camera.ProjectionView);

  game.ExecuteVariableStep(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    // TODO: why is this upside down?
    var mousePos = mouse.NormalisedPosition;

    var targetX = mousePos.X * camera.Size.X - camera.Size.X / 2f;
    var targetY = mousePos.Y * camera.Size.Y - camera.Size.Y / 2f;

    if (mouse.IsButtonDown(MouseButton.Left))
    {
      for (int i = 0; i < 32; i++)
      {
        actors.Add(new Bunny
        {
          Position = new Vector2(targetX, 1 - targetY),
          Velocity = Random.Shared.NextUnitCircle()
        });
      }
    }

    if (mouse.IsButtonDown(MouseButton.Right))
    {
      for (int i = 0; i < 32; i++)
      {
        if (actors.Count > 0)
        {
          actors.RemoveAt(actors.Count - 1);
        }
      }
    }

    graphics.ClearColorBuffer(Color.White);

    using (target.Rent())
    {
      batch.Begin(spriteMaterial);

      foreach (ref var bunny in actors.AsSpan())
      {
        bunny.Update(time.DeltaTime);
        bunny.Draw(batch, sprite);
      }

      batch.DrawText(
        font: font,
        text: $"Bunnies {actors.Count}",
        position: new Vector2(50f, 50f),
        scale: Vector2.One,
        color: Color.Black
      );

      batch.Flush();
    }

    batch.Begin(aberrationMaterial);
    batch.Draw(target.ColorAttachment, Vector2.Zero, Vector2.One);
    batch.Flush();
  });
});

public record struct Bunny
{
  public Vector2 Position;
  public Vector2 Velocity;

  public void Update(TimeDelta deltaTime)
  {
    Position += Velocity * deltaTime * 100f;

    CheckIfOffScreen();
  }

  public void Draw(SpriteBatch batch, Texture sprite)
  {
    batch.Draw(sprite, Position);
  }

  private void CheckIfOffScreen()
  {
    if (Velocity.X < 0f && Position.X < -960f) Position.X = 960f;  // left
    if (Velocity.Y < 0f && Position.Y < -540f) Position.Y = 540f;  // top
    if (Velocity.X > 0f && Position.X > 960f) Position.X  = -960f; // right
    if (Velocity.Y > 0f && Position.Y > 540f) Position.Y  = -540f; // bottom
  }
}
