using Surreal.Graphs;
using Surreal.Objects;

namespace Isaac.Core.Dungeons;

/// <summary>A plan for a dungeon.</summary>
public sealed record DungeonPlan(Random Random)
{
  public SparseGrid<RoomPlan> Rooms { get; init; } = new();

  /// <summary>The current cursor position in the dungeon plan, for placing rooms.</summary>
  public Point2 Cursor = Point2.Zero;
}

/// <summary>Base class for any node in a dungeon plan.</summary>
public abstract record DungeonNode : GraphNode<DungeonNode>
{
  protected virtual DungeonPlan Plan(DungeonPlan plan)
  {
    foreach (var child in Children)
    {
      child.Plan(plan);
    }

    return plan;
  }
}

/// <summary>A blueprint for a <see cref="DungeonPlan"/>.</summary>
public sealed record DungeonBlueprint : DungeonNode, ITemplate<DungeonPlan>
{
  /// <summary>A very simple <see cref="DungeonBlueprint"/> for testing purposes.</summary>
  public static DungeonBlueprint Simple { get; } = new()
  {
    new PlaceRoom(RoomType.Spawn),
    new AdvanceCursor(),
    new PlaceRoom(RoomType.Standard),
    new AdvanceCursor(),
    new PlaceRoom(RoomType.Item),
    new AdvanceCursor(),
    new PlaceRoom(RoomType.Standard),
    new AdvanceCursor(),
    new PlaceRoom(RoomType.Standard),
    new AdvanceCursor(),
    new PlaceRoom(RoomType.Shop),
    new AdvanceCursor(),
    new PlaceRoom(RoomType.Standard),
    new AdvanceCursor(),
    new PlaceRoom(RoomType.Secret),
    new AdvanceCursor(),
    new PlaceRoom(RoomType.Standard),
    new AdvanceCursor(),
    new PlaceRoom(RoomType.Standard),
    new AdvanceCursor(),
    new PlaceRoom(RoomType.Boss),
    new ConnectPathways(0.05f),
  };

  public Seed Seed { get; init; }

  public DungeonPlan Create()
  {
    return Plan(new DungeonPlan(Seed.ToRandom()));
  }
}

/// <summary>Places a room in the dungeon.</summary>
public sealed record PlaceRoom(RoomType Type) : DungeonNode
{
  protected override DungeonPlan Plan(DungeonPlan plan)
  {
    plan.Rooms[plan.Cursor] = new RoomPlan(Type);

    return plan;
  }
}

/// <summary>Advances the cursor in a random open direction.</summary>
public sealed record AdvanceCursor(Axis Axis = Axis.All) : DungeonNode
{
  protected override DungeonPlan Plan(DungeonPlan plan)
  {
    if (Axis.HasFlagFast(Axis.Horizontal))
    {
      plan.Cursor.X += plan.Random.Next(-1, 1);
    }

    if (Axis.HasFlagFast(Axis.Vertical))
    {
      plan.Cursor.Y += plan.Random.Next(-1, 1);
    }

    return plan;
  }
}

/// <summary>Connects the different pathways in a dungeon.</summary>
public sealed record ConnectPathways(float Frequency = 0.1f) : DungeonNode
{
  protected override DungeonPlan Plan(DungeonPlan plan)
  {
    // TODO: implement me

    return plan;
  }
}
