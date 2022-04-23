using Surreal.Timing;

namespace Surreal.Combat.Effects;

/// <summary>A status effect that can be periodically updated.</summary>
public abstract class StatusEffect
{
  public virtual StatusEffectType Type { get; } = StatusEffectType.None;

  public virtual void OnEffectAdded(object target)
  {
  }

  public virtual void OnEffectRemoved(object target)
  {
  }

  public virtual Transition OnEffectUpdate(object target, DeltaTime deltaTime)
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
