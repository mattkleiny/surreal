using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Effects;

// TODO: use a bitset to represent the presence of different 'kinds' of status effects

/// <summary>A managed collection of <see cref="StatusEffect"/>s.</summary>
[DebuggerDisplay("{effects.Count} status effects")]
public sealed class StatusEffectCollection : IEnumerable<StatusEffect>
{
  private readonly LinkedList<StatusEffect> effects = new(); // linked list for fast insertion/removal
  private readonly HierarchicalBitSet effectKinds = new();   // a bit set of which kinds are active in the collection
  private readonly object owner;

  public event Action<StatusEffect>? EffectAdded;
  public event Action<StatusEffect>? EffectRemoved;

  public StatusEffectCollection(object owner)
  {
    this.owner = owner;
  }

  public bool Has(StatusEffectKind kind)
  {
    return effectKinds.Contains(kind.Mask);
  }

  public bool Add(StatusEffect effect)
  {
    effects.AddLast(effect);

    effect.OnEffectAdded(owner);
    EffectAdded?.Invoke(effect);

    return true;
  }

  public bool Remove(StatusEffect effect)
  {
    if (effects.Remove(effect))
    {
      effect.OnEffectRemoved(owner);
      EffectRemoved?.Invoke(effect);

      return true;
    }

    return false;
  }

  public void Update(DeltaTime deltaTime)
  {
    for (var effect = effects.First; effect != null; effect = effect.Next)
    {
      var transition = effect.Value.OnEffectUpdate(owner, deltaTime);
      if (transition == StatusEffect.Transition.Remove)
      {
        effects.Remove(effect);

        effect.Value.OnEffectRemoved(owner);
        EffectRemoved?.Invoke(effect.Value);
      }
    }
  }

  public LinkedList<StatusEffect>.Enumerator GetEnumerator() => effects.GetEnumerator();
  IEnumerator<StatusEffect> IEnumerable<StatusEffect>.GetEnumerator() => effects.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
