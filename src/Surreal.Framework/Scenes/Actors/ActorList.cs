using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surreal.Framework.Scenes.Actors {
  public sealed class ActorList : IReadOnlyList<Actor> {
    private readonly List<Actor> entries = new List<Actor>();
    private readonly Actor?      parent;

    public ActorList(Actor? parent) {
      this.parent = parent;
    }

    public int Count => entries.Count;

    public Actor this[int index] => entries[index];

    public void Add(Actor actor) {
      Debug.Assert(actor.Parent == null, "actor.Parent == null");

      actor.Parent = parent;
      entries.Add(actor);
    }

    public void Remove(Actor actor) {
      Debug.Assert(actor.Parent != null, "actor.Parent != null");

      actor.Parent = null;
      entries.Remove(actor);
    }

    public void Clear() {
      for (var i = 0; i < entries.Count; i++) {
        entries[i].Parent = null;
      }

      entries.Clear();
    }

    public List<Actor>.Enumerator         GetEnumerator() => entries.GetEnumerator();
    IEnumerator<Actor> IEnumerable<Actor>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.              GetEnumerator() => GetEnumerator();
  }
}