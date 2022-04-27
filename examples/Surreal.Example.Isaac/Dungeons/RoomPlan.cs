namespace Isaac.Dungeons;

public enum RoomType
{
  Spawn,
  Standard,
  Shop,
  Boss,
}

public sealed record RoomPlan
{
  public RoomPlan?      Parent   { get; init; } = null;
  public List<RoomPlan> Children { get; init; } = new();
  public Point2         Position { get; set; }  = Point2.Zero;
  public Point2         Size     { get; set; }  = new(256 / 16, 144 / 16);
  public RoomType       Type     { get; set; }  = RoomType.Standard;

  public bool IsConnected => Parent != null || Children.Count > 0;

  public RoomPlan AddChild(Direction direction)
  {
    var child = new RoomPlan
    {
      Parent   = this,
      Position = Position + Size * direction.ToPoint2(),
      Size     = Size,
      Type     = RoomType.Standard,
    };

    Children.Add(child);
    return child;
  }

  public void DrawGizmos(GeometryBatch batch)
  {
    var padding = new Point2(2, 2);

    var outerColor = IsConnected ? Color.Green : Color.Red;
    var innerColor = Type switch
    {
      RoomType.Spawn    => Color.Green,
      RoomType.Standard => Color.White,
      RoomType.Shop     => Color.Yellow,
      RoomType.Boss     => Color.Red,

      _ => throw new ArgumentOutOfRangeException()
    };

    batch.DrawWireQuad(Position, Size, outerColor);
    batch.DrawWireQuad(Position, Size - padding, innerColor);

    foreach (var child in Children)
    {
      child.DrawGizmos(batch);
    }
  }
}

public sealed class RoomPlanGrid
{
  private readonly SparseGrid<RoomPlan> grid = new();
  private readonly LinkedList<RoomPlan> rooms = new();

  public RoomPlan? First => rooms.First?.Value;
  public RoomPlan? Last  => rooms.Last?.Value;

  public RoomPlan? this[Point2 position]
  {
    get => grid[position];
    set => grid[position] = value;
  }

  public RoomPlan? this[int x, int y]
  {
    get => grid[x, y];
    set => grid[x, y] = value;
  }

  public bool IsEmpty(Point2 position)
  {
    return grid[position] == null;
  }

  public void Add(RoomPlan room)
  {
    grid[room.Position] = room;
    rooms.AddLast(room);
  }

  public void Clear()
  {
    grid.Clear();
    rooms.Clear();
  }
}
