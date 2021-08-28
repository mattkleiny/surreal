using Surreal.Mathematics;
using Surreal.Mechanics.Tactical.Attributes;

namespace Isaac.Mechanics
{
  public static class Attributes
  {
    public static AttributeType<int>    Health  { get; } = new(nameof(Health));
    public static AttributeType<int>    Mana    { get; } = new(nameof(Mana));
    public static AttributeType<Normal> Stamina { get; } = new(nameof(Stamina));
  }
}
