using Surreal.Assets;

namespace Surreal.Graphics.Fonts;

/// <summary>A font represented as small bitmaps.</summary>
public sealed class BitmapFont
{
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="BitmapFont"/>s.</summary>
public sealed class BitmapFontLoader : AssetLoader<BitmapFont>
{
  public override ValueTask<BitmapFont> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    throw new NotImplementedException();
  }
}
