using System;

namespace Surreal.Mechanics.Tactical.Combat
{
  public readonly record struct DamageType(string Name)
  {
    public override int GetHashCode()
    {
      return string.GetHashCode(Name, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(DamageType other)
    {
      return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }
  }
}
