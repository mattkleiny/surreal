namespace Surreal.Components;

/// <summary>A group of <see cref="IComponentStorage{T}"/>s.</summary>
internal interface IComponentStorageGroup
{
  IComponentStorage<T> GetOrCreateStorage<T>()
    where T : notnull;
}
