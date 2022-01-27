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

  public PropertyCollection Properties { get; } = new();

  public int Health
  {
    get => Properties.Get(SharedProperties.Health);
    set => Properties.Set(SharedProperties.Health, value.Clamp(0, 99));
  }

  public int Bombs
  {
    get => Properties.Get(SharedProperties.Bombs);
    set => Properties.Set(SharedProperties.Bombs, value.Clamp(0, 99));
  }

  public int Coins
  {
    get => Properties.Get(SharedProperties.Coins);
    set => Properties.Set(SharedProperties.Coins, value.Clamp(0, 99));
  }

  protected override void OnEnable()
  {
    Message.SubscribeAll(this);

    base.OnEnable();
  }

  protected override void OnDisable()
  {
    base.OnDisable();

    Message.UnsubscribeAll(this);
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
