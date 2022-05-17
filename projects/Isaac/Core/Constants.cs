using Surreal.Collections;
using Surreal.Systems;
using Surreal.Systems.Attributes;
using Surreal.Systems.Effects;

namespace Isaac.Core;

/// <summary>Commonly used <see cref="Property{T}"/>s across the project.</summary>
public static class PropertyTypes
{
  public static Property<int> Health      { get; } = new(nameof(Health));
  public static Property<int> MoveSpeed   { get; } = new(nameof(MoveSpeed));
  public static Property<int> AttackSpeed { get; } = new(nameof(AttackSpeed));
  public static Property<int> Range       { get; } = new(nameof(Range));
  public static Property<int> Bombs       { get; } = new(nameof(Bombs));
  public static Property<int> Coins       { get; } = new(nameof(Coins));
}

/// <summary>Commonly used <see cref="AttributeType"/>s across the project.</summary>
public static class AttributeTypes
{
  public static AttributeType Health      { get; } = new(PropertyTypes.Health);
  public static AttributeType MoveSpeed   { get; } = new(PropertyTypes.MoveSpeed);
  public static AttributeType AttackSpeed { get; } = new(PropertyTypes.AttackSpeed);
  public static AttributeType Range       { get; } = new(PropertyTypes.Range);
};

/// <summary>Commonly used <see cref="DamageType"/>s across the project.</summary>
public static class DamageTypes
{
  public static DamageType Standard { get; } = new(nameof(Standard));
  public static DamageType Poison   { get; } = new(nameof(Poison));
}

/// <summary>Commonly used <see cref="StatusEffectType"/>s across the project.</summary>
public static class StatusEffectTypes
{
  public static StatusEffectType Frozen { get; } = new(nameof(Frozen));
  public static StatusEffectType Poison { get; } = new(nameof(Poison));
}
