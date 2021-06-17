using System.Threading.Tasks;
using Surreal.Data;
using Surreal.Data.VFS;

namespace Surreal.Assets {
  public interface IAssetResolver {
    Task<TAsset> GetAsync<TAsset>(Path path);
  }
}