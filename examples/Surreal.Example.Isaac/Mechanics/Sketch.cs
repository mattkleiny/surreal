using Surreal.Mathematics;
using Surreal.Mechanics.Tactical.Attributes;
using Surreal.Mechanics.Tactical.Combat;

namespace Isaac.Mechanics
{
  public static class Attributes
  {
    public static AttributeType<int>    Health  { get; } = new(nameof(Health));
    public static AttributeType<int>    Mana    { get; } = new(nameof(Mana));
    public static AttributeType<Normal> Stamina { get; } = new(nameof(Stamina));
  }

  public static class DamageTypes
  {
    public static DamageType Physical { get; } = new(nameof(Physical));
    public static DamageType Acid     { get; } = new(nameof(Acid));
    public static DamageType Fire     { get; } = new(nameof(Fire));
    public static DamageType Void     { get; } = new(nameof(Void));
  }
}
