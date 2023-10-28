using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using MSBuildProject = Microsoft.Build.Evaluation.Project;

namespace Surreal.Editing.Projects;

/// <summary>
/// A wrapper over an MSBuild project file.
/// </summary>
internal sealed class ProjectFile(MSBuildProject project)
{
  public static ProjectFile Load(string path)
  {
    var fullPath = Path.GetFullPath(path);
    var project = MSBuildProject.FromFile(fullPath, new ProjectOptions
    {
      Interactive = false,
      LoadSettings = ProjectLoadSettings.IgnoreMissingImports,

    });

    return new ProjectFile(project);
  }
}
