namespace Isaac.Dungeons;

// TODO: binary serialization?

public enum TileKind : byte
{
  Floor,
  Wall,
  Door,
  Void,
  Water,
  Spawner
}

public sealed record RoomTemplate(int Width, int Height)
{
  public Grid<TileKind> Tiles { get; } = new(Width, Height);
}
