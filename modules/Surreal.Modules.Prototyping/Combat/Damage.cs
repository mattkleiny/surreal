namespace Surreal.Combat;

/// <summary>Indicates damage to be applied to some other object.</summary>
public record struct Damage(int Amount, DamageType Type)
{
  public static Damage MinValue(DamageType type) => new(int.MinValue, type);
  public static Damage MaxValue(DamageType type) => new(int.MaxValue, type);

  /// <summary>Applies damage to the given receiver.</summary>
  public readonly void ApplyTo(IDamageReceiver receiver)
  {
    receiver.OnDamageReceived(this);
  }

  public override string ToString() => $"{Amount} {Type}";
}

/// <summary>Indicates a type of damage to be applied from one object to another.</summary>
public readonly record struct DamageType(string Name)
{
  public override string ToString() => Name;
}

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

/// <summary>An object capable of receiving <see cref="Damage"/>.</summary>
public interface IDamageReceiver
{
  /// <summary>Indicates the object has received damage from a source.</summary>
  void OnDamageReceived(Damage damage);
}
