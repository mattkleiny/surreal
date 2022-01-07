using Surreal.Timing;

namespace Surreal.Effects;

/// <summary>A status effect that can be periodically updated.</summary>
public abstract record StatusEffect
{
  public virtual void OnEffectAdded(object owner)
  {
  }

  public virtual void OnEffectRemoved(object owner)
  {
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
