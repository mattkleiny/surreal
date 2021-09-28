using System.Threading.Tasks;
using SixLabors.ImageSharp;
using Surreal.Content;
using Surreal.IO;

namespace Surreal.Graphics.Fonts
{
  public sealed class BitmapFont
  {
  }

  public sealed class BitmapFontLoader : AssetLoader<BitmapFont>
  {
    public override async Task<BitmapFont> LoadAsync(Path path, IAssetResolver resolver)
    {
      var image = await resolver.LoadAsset<Image>(path);

      return new BitmapFont();
    }
  }
}
