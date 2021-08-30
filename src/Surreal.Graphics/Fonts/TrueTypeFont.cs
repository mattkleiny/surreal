using System;
using System.Threading.Tasks;
using Surreal.Content;
using Surreal.IO;

namespace Surreal.Graphics.Fonts
{
  public sealed class TrueTypeFont
  {
  }

  public sealed class TrueTypeFontLoader : AssetLoader<TrueTypeFont>
  {
    public override async Task<TrueTypeFont> LoadAsync(Path path, IAssetResolver resolver)
    {
      await using var stream = await path.OpenInputStreamAsync();

      throw new NotImplementedException();
    }
  }
}
