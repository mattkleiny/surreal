using Surreal.Assets;
using Surreal.IO;
using Path = Surreal.IO.Path;

namespace Surreal.Compute.Execution;

/// <summary>A compute program that can be executed on the GPGPU.</summary>
public abstract class ComputeProgram : ComputeResource
{
  public abstract void Execute(int groupsX, int groupsY, int groupsZ);
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="ComputeProgram"/>s.</summary>
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
