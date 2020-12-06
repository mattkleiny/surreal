using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surreal.Framework.Scenes.Actors {
  public sealed class ComponentList : IEnumerable<IActorComponent> {
    private readonly List<IActorComponent> entries = new();
    private readonly Actor?                parent;

    public ComponentList(Actor? parent) {
      this.parent = parent;
    }

    public int Count => entries.Count;

    public IActorComponent this[int index] => entries[index];

    public void Add(IActorComponent component) {
      Debug.Assert(component.Actor == null, "component.Actor == null");

      component.Actor = parent;
      entries.Add(component);
    }

    public void Remove(IActorComponent component) {
      Debug.Assert(component.Actor != null, "component.Actor != null");

      component.Actor = null;
      entries.Remove(component);
    }

    public void Clear() {
      for (var i = 0; i < entries.Count; i++) {
        entries[i].Actor = null;
      }

      entries.Clear();
    }

    public List<IActorComponent>.Enumerator                   GetEnumerator() => entries.GetEnumerator();
    IEnumerator<IActorComponent> IEnumerable<IActorComponent>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                                  GetEnumerator() => GetEnumerator();
  }
}