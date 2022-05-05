namespace Prelude.Graphics;

/// <summary>A single tile in the <see cref="TileMap"/>.</summary>
public sealed record Tile(byte Id) : Enumeration<Tile>
{
  public static Tile Empty { get; } = new(0);
  public static Tile Wall  { get; } = new(1);

  public static bool TryGet(byte id, out Tile result)
  {
    foreach (var tile in All)
    {
      if (tile.Id == id)
      {
        result = tile;
        return true;
      }
    }

    result = default!;
    return false;
  }
}

/// <summary>A tilemap that can be rendered to a <see cref="PixelCanvas"/>.</summary>
public sealed class TileMap
{
  public static TileMap Default { get; } = Create(@"
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
    get
    {
      if (!Tile.TryGet(tiles[x, y], out var tile))
      {
        throw new Exception($"An invalid tile is specified at ({x}, {y})");
      }

      return tile;
    }
    set => tiles[x, y] = value.Id;
  }

  public void Draw(SpanGrid<Color32> pixels)
  {
    for (int y = 0; y < Height; y++)
    for (int x = 0; x < Width; x++)
    {
      pixels[x, y] = tiles[x, y] switch
      {
        0 => Color32.Black,
        1 => Color32.White,
        _ => Color32.Magenta
      };
    }
  }
}
