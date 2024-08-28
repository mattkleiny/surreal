using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Surreal.Build.Targets;
using Surreal.Utilities;
using Task = Microsoft.Build.Utilities.Task;

namespace Surreal.Tasks;

/// <summary>
/// An MSBuild task that builds a package as part of the build pipeline.
/// </summary>
[UsedImplicitly]
public sealed class BuildPackage : Task
{
  [Required] public required string ProjectName { get; set; }
  [Required] public required string OutputDirectory { get; set; }
  [Required] public required PlatformIdentifier TargetPlatform { get; set; }

  public override bool Execute()
  {
    try
    {
      var target = PlatformTarget.Create(TargetPlatform);
      var reporter = new ProgressReporter(Log);
      var configuration = new PackageConfiguration
      {
        Name = ProjectName,
        Icon = "icon.ico",
        OutputDirectory = OutputDirectory,
      };

      target.BuildPackageAsync(configuration, reporter).Wait();

      return true;
    }
    catch (Exception exception)
    {
      Log.LogErrorFromException(exception);

      return false;
    }
  }

  private sealed class ProgressReporter(TaskLoggingHelper log) : IProgressReporter
  {
    public IProgressScope StartOperation(string operation)
    {
      return new ProgressScope(log, operation);
    }

    private sealed class ProgressScope(TaskLoggingHelper log, string operation) : IProgressScope
    {
      public void ReportProgress(float progress)
      {
        log.LogMessage(MessageImportance.Normal, $"{operation}: {progress:P0}");
      }

      public void Dispose()
      {
      }
    }
  }
}
