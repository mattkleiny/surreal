using Task = Microsoft.Build.Utilities.Task;

namespace Surreal.Tasks;

/// <summary>
/// An MSBuild task that bakes assets as part of the build pipeline.
/// </summary>
[UsedImplicitly]
public sealed class BakeAssets : Task
{
  public override bool Execute()
  {
    throw new NotImplementedException();
  }
}
