using Surreal.Attributes;

namespace Isaac.Core;

/// <summary>Commonly used <see cref="AttributeType"/>s across the project.</summary>
public static class Attributes
{
  public static AttributeType Health      { get; } = new(Properties.Health);
  public static AttributeType MoveSpeed   { get; } = new(Properties.MoveSpeed);
  public static AttributeType AttackSpeed { get; } = new(Properties.AttackSpeed);
  public static AttributeType Range       { get; } = new(Properties.Range);
}
