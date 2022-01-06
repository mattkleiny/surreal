using Surreal.IO;

namespace Surreal.Assets;

/// <summary>Context for <see cref="IAssetLoader"/> operations.</summary>
public readonly record struct AssetLoaderContext
{
  public VirtualPath   Path      { get; init; }
  public IAssetManager Manager   { get; init; }
  public Type          AssetType { get; init; }
}

/// <summary>Allows loading assets from storage.</summary>
public interface IAssetLoader
{
  bool CanHandle(AssetLoaderContext context);

  Task<object> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default);
}

/// <summary>Base class for any <see cref="IAssetLoader"/> implementation.</summary>
public abstract class AssetLoader<T> : IAssetLoader
  where T : notnull
{
  public virtual bool CanHandle(AssetLoaderContext context)
  {
    return context.AssetType == typeof(T);
  }

  public abstract Task<T> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default);

  async Task<object> IAssetLoader.LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    return await LoadAsync(context, cancellationToken);
  }
}
