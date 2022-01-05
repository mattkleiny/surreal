using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Graphics.Fonts;

/// <summary>A font represented as small bitmaps.</summary>
public sealed class BitmapFont
{
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="BitmapFont"/>s.</summary>
public sealed class BitmapFontLoader : AssetLoader<BitmapFont>
{
  public override Task<BitmapFont> LoadAsync(VirtualPath path, IAssetContext context, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
