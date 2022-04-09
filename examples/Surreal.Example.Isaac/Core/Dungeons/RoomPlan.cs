namespace Isaac.Core.Dungeons;

/// <summary>Different types of <see cref="RoomPlan"/>s.</summary>
public enum RoomType
{
  Standard,
  Spawn,
  Shop,
  Secret,
  Item,
  Boss,
}

/// <summary>A plan for room in a dungeon.</summary>
public sealed record RoomPlan(RoomType Type);
