using System;
using System.Threading.Tasks;
using Surreal.Content;
using Surreal.IO;

namespace Surreal.Graphics.Fonts
{
  public sealed class BitmapFont
  {
  }

  public sealed class BitmapFontLoader : AssetLoader<BitmapFont>
  {
    public override async Task<BitmapFont> LoadAsync(Path path, IAssetResolver context)
    {
      await using var stream = await path.OpenInputStreamAsync();

      throw new NotImplementedException();
    }
  }
}
