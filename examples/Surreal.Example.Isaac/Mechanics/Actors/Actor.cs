using Isaac.Mechanics.Effects;
using Surreal.Mathematics;
using Surreal.Mechanics.Tactical;
using Surreal.Mechanics.Tactical.Attributes;
using Surreal.Mechanics.Tactical.Effects;
using Surreal.Timing;

namespace Isaac.Mechanics.Actors
{
  public class Actor : IDamageReceiver, IStatusEffectOwner, IFreezable
  {
    private readonly AttributeCollection    attributes;
    private readonly StatusEffectCollection statusEffects;

    public Actor()
    {
      attributes    = new();
      statusEffects = new(this);

      statusEffects.EffectAdded   += OnEffectAdded;
      statusEffects.EffectRemoved += OnEffectRemoved;
    }

    public int Health
    {
      get => attributes.Get(Attributes.Health);
      set => attributes.Set(Attributes.Health, value);
    }

    public int Mana
    {
      get => attributes.Get(Attributes.Mana);
      set => attributes.Set(Attributes.Mana, value);
    }

    public Normal Stamina
    {
      get => attributes.Get(Attributes.Stamina);
      set => attributes.Set(Attributes.Stamina, value);
    }

    public void Update(DeltaTime deltaTime)
    {
      statusEffects.Update(deltaTime);
    }

    public void AddStatusEffect(StatusEffect effect)
    {
      statusEffects.Add(effect);
    }

    public void RemoveStatusEffect(StatusEffect effect)
    {
      statusEffects.Remove(effect);
    }

    public virtual void OnEffectAdded(StatusEffect effect)
    {
    }

    public virtual void OnEffectRemoved(StatusEffect effect)
    {
    }

    public virtual void OnFrozen()
    {
    }

    public virtual void OnUnfrozen()
    {
    }

    void IDamageReceiver.ReceiveDamage(object source, Damage damage)
    {
      Health -= damage.Amount;
    }
  }
}
