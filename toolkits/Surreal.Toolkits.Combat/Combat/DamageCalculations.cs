namespace Surreal.Combat;

/// <summary>Allows calculation of damage from one object to another.</summary>
public delegate void DamageCalculation(Damage baseDamage, ref Damage damage);

/// <summary>Commonly used <see cref="DamageCalculation"/>s.</summary>
public static class DamageCalculations
{
  public static DamageCalculation Additive(int amount)
    => (Damage _, ref Damage finalDamage) => finalDamage.Amount += amount;

  public static DamageCalculation Multiplicative(int amount)
    => (Damage _, ref Damage finalDamage) => finalDamage.Amount *= amount;

  public static DamageCalculation ReplaceWithType(DamageType type)
    => (Damage _, ref Damage finalDamage) => finalDamage.Type = type;
}
