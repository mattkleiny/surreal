using Isaac.Core.Actors.Components;
using Surreal.Attributes;
using Surreal.Effects;

namespace Isaac.Core.Actors;

// character related messages
public readonly record struct CharacterSpawned(Character Character);
public readonly record struct CharacterDamaged(Character Character, Damage Damage);
public readonly record struct CharacterDestroyed(Character Character);
public readonly record struct CharacterGainedStatus(Character Character, StatusEffect Effect);
public readonly record struct CharacterLostStatus(Character Character, StatusEffect Effect);

/// <summary>Different kinds of locomotion states for a <see cref="Character"/>.</summary>
public enum LocomotionState
{
  Normal,
  Flying,
  Stuck,
}

/// <summary>A character <see cref="Actor"/> that can move about the game world and common components.</summary>
public class Character : Actor, IAttributeOwner, IStatusEffectOwner, IDamageReceiver
{
  public Character()
  {
    Health      = 10;
    MoveSpeed   = 4;
    AttackSpeed = 4;
    Range       = 2;

    StatusEffects = new(this);

    StatusEffects.EffectAdded   += OnStatusAdded;
    StatusEffects.EffectRemoved += OnStatusRemoved;
  }

  public ref Transform Transform => ref GetComponent<Transform>();
  public ref RigidBody RigidBody => ref GetComponent<RigidBody>();
  public ref Collider  Collider  => ref GetComponent<Collider>();
  public ref Sprite    Sprite    => ref GetComponent<Sprite>();
  public ref Point2    Position  => ref Transform.Position;
  public ref Vector2   Velocity  => ref RigidBody.Velocity;
  public ref Glyph     Glyph     => ref Sprite.Glyph;

  public IPropertyCollection    Properties    { get; } = new PropertyBag();
  public StatusEffectCollection StatusEffects { get; }

  public int this[AttributeType attribute]
  {
    get => Properties.Get(attribute.Property);
    set => Properties.Set(attribute.Property, value);
  }

  public int Health
  {
    get => Properties.Get(PropertyTypes.Health);
    set => Properties.Set(PropertyTypes.Health, value.Clamp(0, 99));
  }

  public int MoveSpeed
  {
    get => Properties.Get(PropertyTypes.MoveSpeed);
    set => Properties.Set(PropertyTypes.MoveSpeed, value.Clamp(0, 20));
  }

  public int AttackSpeed
  {
    get => Properties.Get(PropertyTypes.AttackSpeed);
    set => Properties.Set(PropertyTypes.AttackSpeed, value.Clamp(0, 20));
  }

  public int Range
  {
    get => Properties.Get(PropertyTypes.Range);
    set => Properties.Set(PropertyTypes.Range, value.Clamp(0, 10));
  }

  public int Bombs
  {
    get => Properties.Get(PropertyTypes.Bombs);
    set => Properties.Set(PropertyTypes.Bombs, value.Clamp(0, 99));
  }

  public int Coins
  {
    get => Properties.Get(PropertyTypes.Coins);
    set => Properties.Set(PropertyTypes.Coins, value.Clamp(0, 99));
  }

  public LocomotionState LocomotionState
  {
    get
    {
      if (StatusEffects.ContainsType(StatusEffectTypes.Frozen))
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
    AddComponent(new RigidBody());
    AddComponent(new Collider());
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

  private void OnStatusAdded(StatusEffect effect)
  {
    Message.Publish(new CharacterGainedStatus(this, effect));
  }

  private void OnStatusRemoved(StatusEffect effect)
  {
    Message.Publish(new CharacterLostStatus(this, effect));
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
