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

  using var texture = await game.Assets.LoadAssetAsync<Texture>("Assets/wabbit_alpha.png");
  using var material = await game.Assets.LoadDefaultSpriteMaterialAsync();
  using var batch = new SpriteBatch(graphics, spriteCount: 8000);

  var camera = new Camera
  {
    Position = new Vector2(0f, 0f),
    Size     = new Vector2(1920f, 1080f)
  };

  var actors = new List<Bunny>();

  material.Properties.Add(Material.DefaultProjectionView, in camera.ProjectionView);

  game.ExecuteVariableStep(_ =>
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
        actors.Add(new Bunny(texture, batch)
        {
          Position = new Vector2(targetX, 1 - targetY)
        });
      }
    }

    graphics.ClearColorBuffer(Color.White);

    batch.Begin(material);

    foreach (ref var bunny in actors.AsSpan())
    {
      bunny.Update();
      bunny.Draw();
    }

    batch.Flush();
  });
});

public record struct Bunny
{
  private readonly Texture sprite;
  private readonly SpriteBatch batch;

  public Bunny(Texture sprite, SpriteBatch batch)
  {
    this.sprite = sprite;
    this.batch  = batch;

    Position = Vector2.Zero;
    Velocity = Random.Shared.NextUnitCircle();
  }

  public Vector2 Position;
  public Vector2 Velocity;

  public void Update()
  {
    Position += Velocity;
  }

  public void Draw()
  {
    batch.Draw(sprite, Position);
  }
}
