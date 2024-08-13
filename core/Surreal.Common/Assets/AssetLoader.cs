using Surreal.IO;

namespace Surreal.Assets;

/// <summary>
/// A callback for reloading an asset.
/// </summary>
public delegate ValueTask ReloadCallback(CancellationToken cancellationToken);

/// <summary>
/// Represents a dependency on some other component <see cref="T"/>.
/// </summary>
public interface IAssetDependency<out T>
{
  /// <summary>
  /// The value of the dependency.
  /// </summary>
  T Value { get; }

  /// <summary>
  /// Adds a callback to be invoked when the dependency's data has changed.
  /// </summary>
  void WhenChanged(Action callback);
}

/// <summary>
/// Context for <see cref="IAssetLoader" /> operations.
/// </summary>
public interface IAssetContext
{
  /// <summary>
  /// The path of the asset being loaded.
  /// </summary>
  VirtualPath Path { get; }

  /// <summary>
  /// The type of the asset being loaded.
  /// </summary>
  Type Type { get; }

  /// <summary>
  /// Loads an asset from the context.
  /// </summary>
  Task<T> LoadAsync<T>(VirtualPath path, CancellationToken cancellationToken = default);

  /// <summary>
  /// Loads a dependent asset from the context.
  /// </summary>
  Task<IAssetDependency<T>> LoadDependencyAsync<T>(VirtualPath path, CancellationToken cancellationToken = default);

  /// <summary>
  /// Adds a reload action to the context.
  /// </summary>
  void WhenPathChanged(ReloadCallback callback);
}

/// <summary>
/// Allows loading assets from storage.
/// </summary>
public interface IAssetLoader
{
  /// <summary>
  /// Determines if the loader can handle the given asset.
  /// </summary>
  bool CanHandle(AssetId id);

  /// <summary>
  /// Loads an asset from storage.
  /// </summary>
  Task<object> LoadAsync(IAssetContext context, CancellationToken cancellationToken);
}

/// <summary>
/// Base class for any <see cref="IAssetLoader"/> implementation.
/// </summary>
public abstract class AssetLoader<T> : IAssetLoader
  where T : notnull
{
  public virtual bool CanHandle(AssetId id)
  {
    return id.Type == typeof(T);
  }

  public abstract Task<T> LoadAsync(IAssetContext context, CancellationToken cancellationToken);

  async Task<object> IAssetLoader.LoadAsync(IAssetContext context, CancellationToken cancellationToken)
  {
    return await LoadAsync(context, cancellationToken);
  }
}
