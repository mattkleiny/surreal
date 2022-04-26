using System.Diagnostics.CodeAnalysis;
using Surreal.IO;

namespace Surreal.Assets;

/// <summary>Represents uniquely some asset type at a given path.</summary>
public readonly record struct AssetId(Type Type, VirtualPath Path)
{
  public override string ToString() => Path.ToString();
}

/// <summary>Allows managing assets.</summary>
public interface IAssetManager : IDisposable
{
  /// <summary>True if hot reloading is enabled across the manager.</summary>
  bool IsHotReloadEnabled { get; }

  /// <summary>Registers a new <see cref="IAssetLoader"/> with the manager.</summary>
  void AddLoader(IAssetLoader loader);

  /// <summary>Loads the given asset from the virtual file system. Returns it if it's already cached.</summary>
  ValueTask<T> LoadAsset<T>(VirtualPath path, CancellationToken cancellationToken = default);

  /// <summary>Registers a listener for changes to the given asset, invoking the given handler when they are detected.</summary>
  IDisposable RegisterForChanges(AssetId id, VirtualPath path, AssetChangedHandler<object> handler);
}

/// <summary>The default <see cref="IAssetManager"/> implementation.</summary>
public sealed class AssetManager : IAssetManager
{
  private readonly List<IAssetLoader> loaders = new();
  private readonly Dictionary<AssetId, object> assetsById = new();

  public void AddLoader(IAssetLoader loader)
  {
    loaders.Add(loader);
  }

  public bool IsHotReloadEnabled { get; set; } = true;

  [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
  public async ValueTask<T> LoadAsset<T>(VirtualPath path, CancellationToken cancellationToken = default)
  {
    var id = new AssetId(typeof(T), path);
    var context = new AssetLoaderContext(id, this);

    if (!TryGetLoader(context, out var loader))
    {
      throw new UnsupportedAssetException($"An unsupported asset type was requested: {context.AssetType.Name}");
    }

    if (!assetsById.TryGetValue(id, out var asset))
    {
      // we'll continue asynchronously on the main thread
      assetsById[id] = asset = await loader.LoadAsync(context, cancellationToken);
    }

    return (T)asset;
  }

  public IDisposable RegisterForChanges(AssetId id, VirtualPath path, AssetChangedHandler<object> handler)
  {
    // dispatches path change events to particular handlers.
    async void OnPathModified(VirtualPath changedPath)
    {
      if (changedPath != path)
      {
        return; // there's a bit of multiplexing going on here
      }

      try
      {
        if (assetsById.TryGetValue(id, out var asset))
        {
          var context = new AssetLoaderContext(id, this);

          assetsById[id] = await handler(context, asset, CancellationToken.None);
        }
      }
      catch (Exception)
      {
        // TODO: handle me? or ignore me?
      }
    }

    // TODO: wire this up inside an 'asset' container, allowing it to manage internal state
    // TODO: free watchers and listeners when the associated asset goes away

    var watcher = path.Watch();

    watcher.Created  += OnPathModified;
    watcher.Modified += OnPathModified;
    watcher.Deleted  += OnPathModified;

    return Disposables.Anonymous(() =>
    {
      watcher.Created  -= OnPathModified;
      watcher.Modified -= OnPathModified;
      watcher.Deleted  -= OnPathModified;

      watcher.Dispose();
    });
  }

  public void Dispose()
  {
    foreach (var asset in assetsById.Values)
    {
      if (asset is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    foreach (var loader in loaders)
    {
      if (loader is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    assetsById.Clear();
    loaders.Clear();
  }

  /// <summary>Attempts to locate a valid loader for the given type.</summary>
  private bool TryGetLoader(AssetLoaderContext context, [NotNullWhen(true)] out IAssetLoader? result)
  {
    for (var i = 0; i < loaders.Count; i++)
    {
      var loader = loaders[i];
      if (loader.CanHandle(context))
      {
        result = loader;
        return true;
      }
    }

    result = default;
    return false;
  }
}

/// <summary>Denotes the given asset type is not supported by the manager.</summary>
public sealed class UnsupportedAssetException : Exception
{
  public UnsupportedAssetException(string message)
    : base(message)
  {
  }
}
