namespace Prelude.Graphics;

/// <summary>A single tile in the <see cref="TileMap"/>.</summary>
public sealed record Tile(byte Id) : Enumeration<Tile>
{
  public static Tile Empty { get; } = new(0);
  public static Tile Wall  { get; } = new(1);

  /// <summary>A look-up of <see cref="Tile"/>s by their associated IDs.</summary>
  public static ImmutableDictionary<byte, Tile> TilesById { get; } = All.ToImmutableDictionary(_ => _.Id);
}

/// <summary>A tilemap that can be rendered to a <see cref="PixelCanvas"/>.</summary>
public sealed class TileMap
{
  public static TileMap CreateDefault() => Create(@"
    ################
    #              #
    #              #
    #              #
    #              #
    #              #
    #              #
    #              #
    ################
  ");

  public static TileMap Create(string pattern)
  {
    var lines = pattern.Trim().Split('\n');
    var maxLength = lines.Max(_ => _.Length);

    var map = new TileMap(maxLength, lines.Length);

    for (var y = 0; y < lines.Length; y++)
    {
      var line = lines[y].Trim();

      for (var x = 0; x < line.Length; x++)
      {
        var character = line[x];

        if (character == '#') map[x, y] = Tile.Wall;
        if (character == ' ') map[x, y] = Tile.Empty;
      }
    }

    return map;
  }

  private readonly Grid<byte> tiles;

  public TileMap(int width, int height)
  {
    Width  = width;
    Height = height;

    tiles = new Grid<byte>(width, height);
  }

  public int Width  { get; }
  public int Height { get; }

  public Tile this[int x, int y]
  {
    get => Tile.TilesById[tiles[x, y]];
    set => tiles[x, y] = value.Id;
  }

  public void Draw(SpanGrid<Color32> pixels)
  {
    var scale = new Vector2(16, 16);

    for (int y = 0; y < Height; y++)
    for (int x = 0; x < Width; x++)
    {
      var tile = tiles[x, y];

      var rect = new Rectangle(
        Left: x * scale.X,
        Top: (y + 1) * scale.Y,
        Right: (x + 1) * scale.X,
        Bottom: y * scale.Y
      ).Clamp(0, 0, pixels.Width - 1, pixels.Height - 1);

      pixels.DrawRectangle(rect, tile switch
      {
        0 => Color32.Black,
        1 => Color32.White,
        _ => Color32.Magenta
      });
    }
  }
}
