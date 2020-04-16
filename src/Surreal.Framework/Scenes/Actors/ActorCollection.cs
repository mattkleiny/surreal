using System.Collections;
using System.Collections.Generic;

namespace Surreal.Framework.Scenes.Actors
{
  public sealed class ActorCollection : IEnumerable<Actor>
  {
    private readonly List<Actor> entries = new List<Actor>();
    private readonly Actor?      parent;

    public ActorCollection(Actor? parent)
    {
      this.parent = parent;
    }

    public int Count => entries.Count;

    public Actor this[int index] => entries[index];

    public void Add(Actor actor)
    {
      actor.Parent = parent;
      entries.Add(actor);
    }

    public void Remove(Actor actor)
    {
      actor.Parent = null;
      entries.Remove(actor);
    }

    public List<Actor>.Enumerator         GetEnumerator() => entries.GetEnumerator();
    IEnumerator<Actor> IEnumerable<Actor>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.              GetEnumerator() => GetEnumerator();
  }
}