namespace Surreal.Components;

/// <summary>A group of <see cref="IComponentStorage{T}"/>s.</summary>
internal interface IComponentStorageGroup
{
  IComponentStorage    UnsafeGetOrCreateStorage(Type type);
  IComponentStorage<T> GetOrCreateStorage<T>() where T : notnull;

  /// <summary>Merges all components from the given storage group into the current group.</summary>
  void UnsafeTransferComponents(ActorId id, IComponentStorageGroup otherGroup);
}
