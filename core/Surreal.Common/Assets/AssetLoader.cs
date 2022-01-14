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

  ValueTask<object> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default);
}

/// <summary>Base class for any <see cref="IAssetLoader"/> implementation.</summary>
public abstract class AssetLoader<T> : IAssetLoader
  where T : notnull
{
  public virtual bool CanHandle(AssetLoaderContext context)
  {
    return context.AssetType == typeof(T);
  }

  public abstract ValueTask<T> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default);

  async ValueTask<object> IAssetLoader.LoadAsync(AssetLoaderContext context, ProgressToken progressToken)
  {
    return await LoadAsync(context, progressToken);
  }
}
