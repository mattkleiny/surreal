using System.Threading.Tasks;
using Surreal.Data;

namespace Surreal.Assets {
  public interface IAssetResolver {
    Task<TAsset> GetAsync<TAsset>(Path path);
  }
}