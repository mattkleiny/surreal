namespace Surreal.Resources;

/// <summary>
/// Provides accesses to application <see cref="IResource"/>s.
/// </summary>
public interface IResourceProvider
{
  /// <summary>
  /// Gets all resources of the specified type.
  /// </summary>
  IEnumerable<T> GetAll<T>()
    where T : class, IResource;

  /// <summary>
  /// Attempts to load a resource with the given id.
  /// </summary>
  bool TryGetById<T>(ulong id, [MaybeNullWhen(false)] out T result)
    where T : class, IResource;

  /// <summary>
  /// Gets a resource by its id.
  /// </summary>
  T GetById<T>(ulong id)
    where T : class, IResource
  {
    if (!TryGetById<T>(id, out var result))
    {
      throw new KeyNotFoundException($"No resource with id {id} was found.");
    }

    return result;
  }

  /// <summary>
  /// Attempts to load a resource with the given path.
  /// </summary>
  bool TryGetByPath<T>(string path, [MaybeNullWhen(false)] out T result)
    where T : class, IResource;

  /// <summary>
  /// Gets a resource by its name.
  /// </summary>
  T GetByPath<T>(string path)
    where T : class, IResource
  {
    if (!TryGetByPath<T>(path, out var result))
    {
      throw new KeyNotFoundException($"No resource with path {path} was found.");
    }

    return result;
  }
}
