using Surreal.Attributes;
using Surreal.Effects;

namespace Avventura.Core.Actors;

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
  Swimming,
}

/// <summary>A character <see cref="Actor"/> that can move about the game world and common components.</summary>
public class Character : Actor, IAttributeOwner, IStatusEffectOwner, IDamageReceiver
{
  private IConsoleDisplay? display;

  public Character()
  {
    StatusEffects = new(this);

    StatusEffects.EffectAdded   += OnStatusEffectAdded;
    StatusEffects.EffectRemoved += OnStatusEffectRemoved;
  }

  public Point2 Position { get; set; }
  public Glyph  Glyph    { get; set; } = new('█', ConsoleColor.Red);

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

  public int Toxin
  {
    get => Properties.Get(PropertyTypes.Toxin);
    set => Properties.Set(PropertyTypes.Toxin, value.Clamp(0, 99));
  }

  public int Coins
  {
    get => Properties.Get(PropertyTypes.Coins);
    set => Properties.Set(PropertyTypes.Coins, value.Clamp(0, 99));
  }

  public LocomotionState LocomotionState
  {
    get { return LocomotionState.Normal; }
  }

  protected override void OnAwake()
  {
    base.OnAwake();

    display = Services?.GetService<IConsoleDisplay>();
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

  protected override void OnDraw(DeltaTime time)
  {
    base.OnDraw(time);

    display?.Draw(Position.X, Position.Y, Glyph);
  }

  private void OnStatusEffectAdded(StatusEffect effect)
  {
    Message.Publish(new CharacterGainedStatus(this, effect));
  }

  private void OnStatusEffectRemoved(StatusEffect effect)
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
