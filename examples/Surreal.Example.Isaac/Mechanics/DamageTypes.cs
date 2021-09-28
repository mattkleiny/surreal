using Surreal.Mechanics.Tactical.Combat;

namespace Isaac.Mechanics
{
  public static class DamageTypes
  {
    public static DamageType Physical { get; } = new(nameof(Physical));
    public static DamageType Acid     { get; } = new(nameof(Acid));
    public static DamageType Fire     { get; } = new(nameof(Fire));
    public static DamageType Void     { get; } = new(nameof(Void));
  }
}
