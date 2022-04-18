using JetBrains.Annotations;
using Task = Microsoft.Build.Utilities.Task;

namespace Surreal;

/// <summary>A <see cref="Task"/> that compiles assets.</summary>
[UsedImplicitly]
public sealed class CompileAssets : Task
{
  public string Path { get; set; } = string.Empty;

  public override bool Execute()
  {
    Log.LogMessage($"Building assets in \"{Path}\"");

    return true;
  }
}
