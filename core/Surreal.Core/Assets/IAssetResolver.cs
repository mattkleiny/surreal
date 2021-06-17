using System.Threading.Tasks;
using Surreal.IO;

namespace Surreal.Assets {
  public interface IAssetResolver {
    Task<TAsset> GetAsync<TAsset>(Path path);
  }
}