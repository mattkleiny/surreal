namespace Surreal.Mechanics.Tactical.Combat
{
  public interface IDamageReceiver
  {
    void ReceiveDamage(in DamagePacket damage);
  }
}
