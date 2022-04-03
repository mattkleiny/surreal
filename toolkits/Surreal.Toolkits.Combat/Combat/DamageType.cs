namespace Surreal.Combat;

/// <summary>Indicates a type of damage to be applied from one object to another.</summary>
public readonly record struct DamageType(string Name)
{
  public static DamageType Standard { get; } = new(nameof(Standard));
}
