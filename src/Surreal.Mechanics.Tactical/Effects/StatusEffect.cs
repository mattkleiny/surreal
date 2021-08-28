using System;
using System.Collections.Generic;
using Surreal.Timing;

namespace Surreal.Mechanics.Tactical.Effects
{
  public interface IStatusEffectOwner
  {
    void AddStatusEffect(StatusEffect effect);
    void RemoveStatusEffect(StatusEffect effect);
  }

  public class StatusEffectCollection : IStatusEffectOwner
  {
    private readonly List<StatusEffect> effects = new();
    private readonly object             owner;

    public event Action<StatusEffect>? EffectAdded;
    public event Action<StatusEffect>? EffectRemoved;

    public StatusEffectCollection(object owner)
    {
      this.owner = owner;
    }

    public void Add(StatusEffect effect)
    {
      effects.Add(effect);
      effect.OnAdded(owner);

      EffectAdded?.Invoke(effect);
    }

    public void Remove(StatusEffect effect)
    {
      if (effects.Remove(effect))
      {
        effect.OnRemoved(owner);
        EffectRemoved?.Invoke(effect);
      }
    }

    public void Update(DeltaTime deltaTime)
    {
      for (var i = effects.Count - 1; i >= 0; i--)
      {
        var effect     = effects[i];
        var transition = effect.OnUpdate(owner, deltaTime);

        if (transition == StatusEffect.Transition.Remove)
        {
          effects.RemoveAt(i);

          effect.OnRemoved(owner);
          EffectRemoved?.Invoke(effect);
        }
      }
    }

    void IStatusEffectOwner.AddStatusEffect(StatusEffect effect)    => Add(effect);
    void IStatusEffectOwner.RemoveStatusEffect(StatusEffect effect) => Remove(effect);
  }

  public abstract record StatusEffect
  {
    public virtual void OnAdded(object owner)
    {
    }

    public virtual void OnRemoved(object owner)
    {
    }

    public virtual Transition OnUpdate(object owner, DeltaTime deltaTime)
    {
      return Transition.Continue;
    }

    public enum Transition
    {
      Continue,
      Remove
    }
  }

  public abstract record TimedStatusEffect(TimeSpan Frequency, TimeSpan Duration) : StatusEffect
  {
    private Timer frequencyTimer = new(Frequency);
    private Timer durationTimer  = new(Duration);

    public override void OnAdded(object owner)
    {
      base.OnAdded(owner);

      frequencyTimer.Reset();
      durationTimer.Reset();
    }

    public sealed override Transition OnUpdate(object owner, DeltaTime deltaTime)
    {
      if (frequencyTimer.Tick(deltaTime))
      {
        OnTick(owner, deltaTime);
      }

      if (durationTimer.Tick(deltaTime))
      {
        return Transition.Remove;
      }

      return Transition.Continue;
    }

    protected virtual void OnTick(object owner, DeltaTime deltaTime)
    {
    }
  }
}
