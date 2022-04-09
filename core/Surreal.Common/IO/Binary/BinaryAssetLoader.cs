using Surreal.Assets;

namespace Surreal.IO.Binary;

/// <summary>Loads templated objects from JSON.</summary>
public sealed class BinaryAssetLoader<T> : AssetLoader<T>
  where T : notnull
{
  public override async ValueTask<T> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    return await context.Path.DeserializeBinaryAsync<T>();
  }
}
