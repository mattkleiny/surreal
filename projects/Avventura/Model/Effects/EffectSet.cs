using System.Collections;
using System.Collections.Generic;
using Avventura.Model.Actors;
using Surreal.Timing;

namespace Avventura.Model.Effects {
  public class EffectSet : IEnumerable<Effect> {
    private readonly List<Effect> effects = new List<Effect>();

    public void Add(Effect effect)    => effects.Add(effect);
    public void Remove(Effect effect) => effects.Remove(effect);

    public void Tick(DeltaTime deltaTime, Character character) {
      for (var i = effects.Count - 1; i >= 0; i--) {
        switch (effects[i].Tick(deltaTime, character)) {
          case EffectStatus.Remove:
            effects.RemoveAt(i);
            break;
        }
      }
    }

    public List<Effect>.Enumerator          GetEnumerator() => effects.GetEnumerator();
    IEnumerator<Effect> IEnumerable<Effect>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                GetEnumerator() => GetEnumerator();
  }
}