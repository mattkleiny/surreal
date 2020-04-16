using System.Threading.Tasks;
using Surreal.Graphics.Fonts;

namespace Surreal.Assets
{
  public static class AssetExtensions
  {
    public static Task<BitmapFont> LoadDefaultFontAsync(this IAssetResolver manager)
      => manager.GetAsync<BitmapFont>("resx://Surreal.Jam.Resources.Fonts.Default.fnt");
  }
}
