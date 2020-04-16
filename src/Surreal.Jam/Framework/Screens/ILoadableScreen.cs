using System.Threading.Tasks;
using Surreal.Assets;

namespace Surreal.Framework.Screens
{
  public interface ILoadableScreen : IScreen
  {
    AssetManager Assets { get; }

    Task LoadInBackgroundAsync(IAssetResolver assets);
  }
}
