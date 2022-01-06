using Surreal.Combat;

namespace Isaac.Actors;

/// <summary>An anonymous character <see cref="Actor"/> that can move about the game world.</summary>
public class Character : Actor, IDamageReceiver
{
  public Character()
  {
    AddComponent(new Transform());
    AddComponent(new Sprite());
    AddComponent(new Statistics());
  }

  public ref Transform  Transform  => ref GetComponent<Transform>();
  public ref Sprite     Sprite     => ref GetComponent<Sprite>();
  public ref Statistics Statistics => ref GetComponent<Statistics>();

  void IDamageReceiver.OnDamageReceived(Damage damage)
  {
    Statistics.Health -= damage.Amount;

    if (Statistics.Health <= 0)
    {
      Destroy();
    }
  }
}
