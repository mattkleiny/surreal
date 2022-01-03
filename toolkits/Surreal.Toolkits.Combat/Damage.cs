namespace Surreal;

/// <summary>Indicates damage to be applied to some other object.</summary>
public record struct Damage(int Amount, DamageType Type, object? UserData)
{
  /// <summary>A callback for performing damage calculations.</summary>
  public static DamageCalculation? Calculation { get; set; }

  /// <summary>Calculates damage to be applied to an object via the <see cref="Calculation"/> callbacks.</summary>
  public static Damage Calculate(int damage, DamageType type, object? userData = null)
  {
    var baseDamage  = new Damage(damage, type, userData);
    var finalDamage = baseDamage;

    if (Calculation == null)
    {
      return finalDamage;
    }

    Calculation.Invoke(baseDamage, ref finalDamage);

    return finalDamage;
  }
}
