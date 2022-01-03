using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Graphics.Fonts;

public sealed class BitmapFont
{
}

public sealed class BitmapFontLoader : AssetLoader<BitmapFont>
{
  public override Task<BitmapFont> LoadAsync(VirtualPath path, IAssetContext context)
  {
    throw new NotImplementedException();
  }
}
