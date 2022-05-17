using Surreal.TileMaps;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Tile Maps",
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

  using var sprite = await game.Assets.LoadAssetAsync<Texture>("Assets/example_tile.png");
  using var material = await game.Assets.LoadPaletteShiftEffectAsync();
  using var batch = new SpriteBatch(graphics);

  // set-up the color palette
  var palette = await game.Assets.LoadPaletteAsync(BuiltInPalette.Demichrome4);

  material.Palette = palette;

  // set-up a basic tilemap
  var tileMap = new TileMap<Tile>(16, 9);
  var tileSize = new Vector2(16, 16);

  // set-up a basic orthographic projection
  var camera = new Camera
  {
    Position = new Vector2(-256f / 2f, -144 / 2f),
    Size     = new Vector2(256, 144)
  };

  material.Locals.SetProperty(MaterialProperty.ProjectionView, in camera.ProjectionView);

  void RandomizeTileMap()
  {
    var random = Random.Shared;

    tileMap.Tiles.Fill(Tile.Empty);

    for (var y = 0; y < tileMap.Height; y++)
    for (var x = 0; x < tileMap.Width; x++)
    {
      if (random.NextBool())
      {
        tileMap.Tiles[x, y] = Tile.Filled;
      }
    }
  }

  RandomizeTileMap();

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    if (keyboard.IsKeyPressed(Key.Space))
    {
      RandomizeTileMap();
    }

    var mousePos = mouse.NormalisedPosition * camera.Size;

    graphics.ClearColorBuffer(palette[^1]);

    batch.Begin(material);

    tileMap.Draw(batch, Vector2.Zero, tileSize, mousePos, (tile, rect) =>
    {
      if (tile == Tile.Filled)
      {
        // TODO: why is this upside down?
        if (rect.Contains(mousePos with { Y = camera.Size.Y - mousePos.Y }))
        {
          return (sprite, Color.Yellow);
        }

        return (sprite, Color.White);
      }

      return (TextureRegion.Empty, Color.White);
    });

    batch.Flush();
  });
});

public readonly record struct Tile(ushort Id)
{
  private static int nextId = 0;
  private static ushort NextId() => (ushort) Interlocked.Increment(ref nextId);

  public static Tile Empty  { get; } = new(Id: NextId());
  public static Tile Filled { get; } = new(Id: NextId());
}
