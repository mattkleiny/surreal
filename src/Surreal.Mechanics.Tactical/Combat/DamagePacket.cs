using System.Numerics;

namespace Surreal.Mechanics.Tactical.Combat
{
  public record struct DamagePacket(Damage Damage, object Source, object Target, Vector2 Direction);
}
