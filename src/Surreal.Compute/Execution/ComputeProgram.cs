using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Compute.Execution {
  public abstract class ComputeProgram : ComputeResource {
    public abstract void Execute(int groupsX, int groupsY, int groupsZ);

    public sealed class Loader : AssetLoader<ComputeProgram> {
      private readonly IComputeDevice device;

      public Loader(IComputeDevice device) {
        this.device = device;
      }

      public override async Task<ComputeProgram> LoadAsync(Path path, IAssetLoaderContext context) {
        var raw = await path.ReadAllBytesAsync();

        return device.Backend.CreateProgram(raw);
      }
    }
  }
}