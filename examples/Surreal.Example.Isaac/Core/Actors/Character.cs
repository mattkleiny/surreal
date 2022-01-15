using Isaac.Core.Actors.Components;
using Surreal.Combat;

namespace Isaac.Core.Actors;

/// <summary>A character <see cref="Actor"/> that can move about the game world and common components.</summary>
public class Character : Actor, IDamageReceiver
{
  public Character(IActorContext context)
    : base(context)
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
