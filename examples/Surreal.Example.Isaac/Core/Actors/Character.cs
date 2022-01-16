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

  public PropertyCollection PropertyBag { get; } = new();

  public int Health
  {
    get => PropertyBag.Get(Properties.Health);
    set => PropertyBag.Set(Properties.Health, value.Clamp(0, 99));
  }

  public int Bombs
  {
    get => PropertyBag.Get(Properties.Bombs);
    set => PropertyBag.Set(Properties.Bombs, value.Clamp(0, 99));
  }

  public int Coins
  {
    get => PropertyBag.Get(Properties.Coins);
    set => PropertyBag.Set(Properties.Coins, value.Clamp(0, 99));
  }

  void IDamageReceiver.OnDamageReceived(Damage damage)
  {
    Health -= damage.Amount;

    if (Health <= 0)
    {
      Destroy();
    }
  }
}
