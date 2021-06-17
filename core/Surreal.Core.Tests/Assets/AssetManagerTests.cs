using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.IO;
using Xunit;

namespace Surreal.Core.Assets {
  public class AssetManagerTests {
    [Fact]
    public async Task it_should_cache_assets_between_loads() {
      var loader  = new TestAssetLoader();
      var manager = new AssetManager(new[] {loader});

      var asset1 = await manager.GetOrLoadAsync<string>("./test.asset");
      var asset2 = await manager.GetOrLoadAsync<string>("./test.asset");

      Assert.Equal(asset1, asset2);
      Assert.Equal(1, loader.LoadCount);
    }

    private sealed class TestAssetLoader : AssetLoader<string> {
      public int LoadCount { get; set; }

      public override Task<string> LoadAsync(Path path, IAssetLoaderContext context) {
        LoadCount++;

        return Task.FromResult("Hello, World!");
      }
    }
  }
}