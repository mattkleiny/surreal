namespace Surreal;

/// <summary>An object capable of receiving <see cref="Damage"/>.</summary>
public interface IDamageReceiver
{
  /// <summary>Indicates the object has received damage from a source.</summary>
  void OnDamageReceived(Damage damage);
}
