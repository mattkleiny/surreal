namespace Surreal.Components;

/// <summary>A sparsely packed <see cref="IComponentStorage{T}"/>.</summary>
public sealed class SparseComponentStorage<T> : IComponentStorage<T>
  where T : notnull, new()
{
  public ref T GetOrCreateComponent(ActorId id, Optional<T> prototype)
  {
    throw new NotImplementedException();
  }

  public ref T GetComponent(ActorId id)
  {
    throw new NotImplementedException();
  }

  public ref T AddComponent(ActorId id, Optional<T> prototype)
  {
    throw new NotImplementedException();
  }

  public bool RemoveComponent(ActorId id)
  {
    throw new NotImplementedException();
  }
}
