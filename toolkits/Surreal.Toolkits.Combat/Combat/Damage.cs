namespace Surreal.Combat;

/// <summary>Indicates damage to be applied to some other object.</summary>
public record struct Damage(int Amount, DamageType Type, object? UserData = null)
{
  public static Damage MinValue(DamageType type) => new(int.MinValue, type);
  public static Damage MaxValue(DamageType type) => new(int.MaxValue, type);

  /// <summary>A callback for performing damage calculations.</summary>
  public static DamageCalculation? Calculation { get; set; }

  /// <summary>Calculates damage to be applied to an object via the <see cref="Calculation"/> callbacks.</summary>
  public static Damage Calculate(int damage, DamageType? type = null, object? userData = null)
  {
    var baseDamage = new Damage(damage, type ?? DamageType.Standard, userData);
    var finalDamage = baseDamage;

    if (Calculation == null)
    {
      return finalDamage;
    }

    Calculation.Invoke(baseDamage, ref finalDamage);

    return finalDamage;
  }

  /// <summary>Applies damage to the given receiver.</summary>
  public void ApplyTo(IDamageReceiver receiver)
  {
    receiver.OnDamageReceived(this);
  }
}
