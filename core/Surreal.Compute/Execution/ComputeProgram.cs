using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Compute.Execution;

/// <summary>A compute program that can be executed on the GPGPU.</summary>
public abstract class ComputeProgram : ComputeResource
{
  public abstract void Execute(uint groupsX, uint groupsY, uint groupsZ);
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="ComputeProgram"/>s.</summary>
[RegisterService(typeof(IAssetLoader))]
public sealed class ComputeProgramLoader : AssetLoader<ComputeProgram>
{
  private readonly IComputeDevice device;

  public ComputeProgramLoader(IComputeDevice device)
  {
    this.device = device;
  }

  public override async Task<ComputeProgram> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    var raw = await context.Path.ReadAllBytesAsync();

    return device.CreateProgram(raw);
  }
}
