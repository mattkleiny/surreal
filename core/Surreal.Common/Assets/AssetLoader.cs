using Surreal.IO;

namespace Surreal.Assets;

/// <summary>Context for <see cref="IAssetLoader"/> operations.</summary>
public readonly record struct AssetLoaderContext(AssetId Id, IAssetManager Manager)
{
  public Type        AssetType => Id.Type;
  public VirtualPath Path      => Id.Path;
}

/// <summary>Allows loading assets from storage.</summary>
public interface IAssetLoader
{
  bool CanHandle(AssetLoaderContext context);

  ValueTask<object> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default);
}

/// <summary>Base class for any <see cref="IAssetLoader"/> implementation.</summary>
public abstract class AssetLoader<T> : IAssetLoader
  where T : notnull
{
  public virtual bool CanHandle(AssetLoaderContext context)
  {
    return context.AssetType == typeof(T);
  }

  public abstract ValueTask<T> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default);

  async ValueTask<object> IAssetLoader.LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    return await LoadAsync(context, cancellationToken);
  }
}
