using System.Collections;
using System.Collections.Generic;

namespace Surreal.Framework.Scenes.Actors {
  public sealed class ComponentCollection : IEnumerable<IActorComponent> {
    private readonly List<IActorComponent> entries = new List<IActorComponent>();
    private readonly Actor?                parent;

    public ComponentCollection(Actor? parent) {
      this.parent = parent;
    }

    public int Count => entries.Count;

    public IActorComponent this[int index] => entries[index];

    public void Add(IActorComponent component) {
      component.Actor = parent;
      entries.Add(component);
    }

    public void Remove(IActorComponent component) {
      component.Actor = null;
      entries.Remove(component);
    }

    public List<IActorComponent>.Enumerator                   GetEnumerator() => entries.GetEnumerator();
    IEnumerator<IActorComponent> IEnumerable<IActorComponent>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                                  GetEnumerator() => GetEnumerator();
  }
}