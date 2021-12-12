using System.Collections;

namespace Surreal.Aspects;

/// <summary>Enumerates entities that match a particular <see cref="Aspect"/>.</summary>
public struct AspectEnumerator : IEnumerator<ActorId>, IEnumerable<ActorId>
{
  // TODO: implement me

  private readonly IEnumerator<ActorId> enumerator;

  public AspectEnumerator(IEnumerator<ActorId> enumerator)
  {
    this.enumerator = enumerator;
  }

  public ActorId     Current => enumerator.Current;
  object IEnumerator.Current => Current;

  public bool MoveNext() => enumerator.MoveNext();
  public void Reset()    => enumerator.Reset();
  public void Dispose()  => enumerator.Dispose();

  public AspectEnumerator                   GetEnumerator() => this;
  IEnumerator<ActorId> IEnumerable<ActorId>.GetEnumerator() => GetEnumerator();
  IEnumerator IEnumerable.                  GetEnumerator() => GetEnumerator();
}