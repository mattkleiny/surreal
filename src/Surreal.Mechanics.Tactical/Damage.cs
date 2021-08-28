using System;

namespace Surreal.Mechanics.Tactical
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

  public delegate void DamageContributor(object source, object target, ref Damage damage);

  public record struct Damage(int Amount, DamageType Type)
  {
    public static event DamageContributor? Calculating;

    public static Damage Calculate(Damage baseDamage, object source, object target)
    {
      Calculating?.Invoke(source, target, ref baseDamage);

      return baseDamage;
    }

    public void ApplyTo(object target, object source)
    {
      if (target is IDamageReceiver receiver)
      {
        receiver.ReceiveDamage(source, this);
      }
    }
  }

  public interface IDamageReceiver
  {
    void ReceiveDamage(object source, Damage damage);
  }
}
