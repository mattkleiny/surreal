using Surreal.Attributes;
using Surreal.Effects;

namespace Avventura.Core;

/// <summary>Commonly used <see cref="Property{T}"/>s across the project.</summary>
public static class PropertyTypes
{
  public static Property<int> Health { get; } = new(nameof(Health));
  public static Property<int> Toxin  { get; } = new(nameof(Toxin));
  public static Property<int> Coins  { get; } = new(nameof(Coins));
}

/// <summary>Commonly used <see cref="AttributeType"/>s across the project.</summary>
public static class AttributeTypes
{
  public static AttributeType Health { get; } = new(PropertyTypes.Health);
  public static AttributeType Toxin  { get; } = new(PropertyTypes.Toxin);
};

/// <summary>Commonly used <see cref="DamageType"/>s across the project.</summary>
public static class DamageTypes
{
  public static DamageType Standard { get; } = new(nameof(Standard));
  public static DamageType Toxin    { get; } = new(nameof(Toxin));
}
