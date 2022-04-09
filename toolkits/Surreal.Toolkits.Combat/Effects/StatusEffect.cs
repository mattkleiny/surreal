using Surreal.Timing;

namespace Surreal.Effects;

/// <summary>A status effect that can be periodically updated.</summary>
public abstract class StatusEffect
{
  public virtual void OnEffectAdded(object owner)
  {
    Message.Publish(new StatusEffectAdded(owner, this));
  }

  public virtual void OnEffectRemoved(object owner)
  {
    Message.Publish(new StatusEffectRemoved(owner, this));
  }

  public virtual Transition OnEffectUpdate(object owner, DeltaTime deltaTime)
  {
    return Transition.Continue;
  }

  /// <summary>Possible transitions for the status effect.</summary>
  public enum Transition
  {
    Continue,
    Remove,
  }
}
