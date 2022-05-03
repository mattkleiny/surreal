namespace Surreal.Mechanics.Combat.Effects;

/// <summary>Indicates a type of status effect.</summary>
public readonly record struct StatusEffectType(string Name)
{
  public static StatusEffectType None { get; } = new(nameof(None));
}
