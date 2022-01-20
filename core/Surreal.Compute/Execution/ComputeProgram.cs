using Surreal.Assets;

namespace Surreal.Compute.Execution;

/// <summary>A compute program that can be executed on the GPGPU.</summary>
public abstract class ComputeProgram : ComputeResource
{
  public abstract void Execute(uint groupsX, uint groupsY, uint groupsZ);
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="ComputeProgram"/>s.</summary>
public sealed class ComputeProgramLoader : AssetLoader<ComputeProgram>
{
  private readonly IComputeServer server;

  public ComputeProgramLoader(IComputeServer server)
  {
    this.server = server;
  }

  public override ValueTask<ComputeProgram> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    throw new NotImplementedException();
  }
}
