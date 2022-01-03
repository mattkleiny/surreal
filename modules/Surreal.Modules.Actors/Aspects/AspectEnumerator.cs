using System.Collections;

namespace Surreal.Aspects;

/// <summary>Enumerates entities that match a particular <see cref="Aspect"/>.</summary>
public struct AspectEnumerator : IEnumerator<ActorId>, IEnumerable<ActorId>
{
  // TODO: implement me

  public ActorId     Current => throw new NotImplementedException();
  object IEnumerator.Current => Current;

  public bool MoveNext() => throw new NotImplementedException();
  public void Reset()    => throw new NotImplementedException();
  public void Dispose()  => throw new NotImplementedException();

  public AspectEnumerator                   GetEnumerator() => this;
  IEnumerator<ActorId> IEnumerable<ActorId>.GetEnumerator() => GetEnumerator();
  IEnumerator IEnumerable.                  GetEnumerator() => GetEnumerator();
}
