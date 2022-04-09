using Isaac.Core.Actors.Components;
using Surreal.Attributes;
using Surreal.Combat;
using Surreal.Effects;

namespace Isaac.Core.Actors;

public readonly record struct CharacterSpawned(Character Character);
public readonly record struct CharacterDamaged(Character Character, Damage Damage);
public readonly record struct CharacterDestroyed(Character Character);
public readonly record struct CharacterGainedStatusEffect(Character Character, StatusEffect Effect);
public readonly record struct CharacterLostStatusEffect(Character Character, StatusEffect Effect);

/// <summary>Different kinds of locomotion states for a <see cref="Character"/>.</summary>
public enum LocomotionState
{
  Normal,
  Flying,
  Stuck,
}

/// <summary>A character <see cref="Actor"/> that can move about the game world and common components.</summary>
public class Character : Actor, IAttributeOwner, IDamageReceiver, IStatusEffectOwner
{
  public Character()
  {
    StatusEffects = new(this);

    StatusEffects.EffectAdded += OnStatusEffectAdded;
    StatusEffects.EffectRemoved += OnStatusEffectRemoved;
  }

  public ref Transform Transform => ref GetComponent<Transform>();
  public ref Sprite    Sprite    => ref GetComponent<Sprite>();
  public ref Vector2   Position  => ref Transform.Position;
  public ref Vector2   Scale     => ref Transform.Scale;
  public ref float     Rotation  => ref Transform.Rotation;
  public ref Color     Tint      => ref Sprite.Tint;

  public IPropertyCollection    PropertyBag   { get; } = new PropertyBag();
  public StatusEffectCollection StatusEffects { get; }

  public int this[AttributeType attribute]
  {
    get => PropertyBag.Get(attribute.Property);
    set => PropertyBag.Set(attribute.Property, value);
  }

  public int Health
  {
    get => PropertyBag.Get(Properties.Health);
    set => PropertyBag.Set(Properties.Health, value.Clamp(0, 99));
  }

  public int MoveSpeed
  {
    get => PropertyBag.Get(Properties.MoveSpeed);
    set => PropertyBag.Set(Properties.MoveSpeed, value.Clamp(0, 99));
  }

  public int AttackSpeed
  {
    get => PropertyBag.Get(Properties.AttackSpeed);
    set => PropertyBag.Set(Properties.AttackSpeed, value.Clamp(0, 99));
  }

  public int Range
  {
    get => PropertyBag.Get(Properties.Range);
    set => PropertyBag.Set(Properties.Range, value.Clamp(0, 99));
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

  public LocomotionState LocomotionState
  {
    get
    {
      if (StatusEffects.Has(StatusEffectKinds.Frozen))
      {
        return LocomotionState.Stuck;
      }

      return LocomotionState.Normal;
    }
  }

  protected override void OnAwake()
  {
    base.OnAwake();

    AddComponent(new Transform());
    AddComponent(new Sprite());
  }

  protected override void OnStart()
  {
    base.OnStart();

    Message.Publish(new CharacterSpawned(this));
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

  protected override void OnUpdate(DeltaTime deltaTime)
  {
    base.OnUpdate(deltaTime);

    StatusEffects.Update(deltaTime);
  }

  private void OnStatusEffectAdded(StatusEffect effect)
  {
    Message.Publish(new CharacterGainedStatusEffect(this, effect));
  }

  private void OnStatusEffectRemoved(StatusEffect effect)
  {
    Message.Publish(new CharacterLostStatusEffect(this, effect));
  }

  void IDamageReceiver.OnDamageReceived(Damage damage)
  {
    Health -= damage.Amount;

    if (Health > 0)
    {
      Message.Publish(new CharacterDamaged(this, damage));
    }
    else
    {
      Message.Publish(new CharacterDestroyed(this));

      Destroy();
    }
  }
}
