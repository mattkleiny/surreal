using System.Numerics;

namespace Surreal.Mechanics.Tactical.Combat
{
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
        receiver.ReceiveDamage(new DamagePacket(this, source, target, Vector2.Zero));
      }
    }
  }
}
