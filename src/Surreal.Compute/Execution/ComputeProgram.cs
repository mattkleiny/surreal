using System.Threading.Tasks;
using Surreal.Content;
using Surreal.IO;

namespace Surreal.Compute.Execution
{
  public abstract class ComputeProgram : ComputeResource
  {
    public abstract void Execute(int groupsX, int groupsY, int groupsZ);
  }

  public sealed class ComputeProgramLoader : AssetLoader<ComputeProgram>
  {
    private readonly IComputeDevice device;

    public ComputeProgramLoader(IComputeDevice device)
    {
      this.device = device;
    }

    public override async Task<ComputeProgram> LoadAsync(Path path, IAssetResolver resolver)
    {
      var raw = await path.ReadAllBytesAsync();

      return device.CreateProgram(raw);
    }
  }
}
