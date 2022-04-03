using Surreal.Timing;

namespace Surreal.Effects;

/// <summary>A managed collection of <see cref="StatusEffect"/>s.</summary>
public sealed class StatusEffectCollection : IEnumerable<StatusEffect>
{
  private readonly LinkedList<StatusEffect> effects = new(); // linked list for fast insertion/removal
  private readonly object owner;

  public StatusEffectCollection(object owner)
  {
    this.owner = owner;
  }

  public bool Add(StatusEffect effect)
  {
    effects.AddLast(effect);
    effect.OnEffectAdded(owner);

    return true;
  }

  public bool Remove(StatusEffect effect)
  {
    if (effects.Remove(effect))
    {
      effect.OnEffectRemoved(owner);
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
      }
    }
  }

  public LinkedList<StatusEffect>.Enumerator GetEnumerator()
  {
    return effects.GetEnumerator();
  }

  IEnumerator<StatusEffect> IEnumerable<StatusEffect>.GetEnumerator()
  {
    return effects.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
