namespace Surreal.Editing.Projects;

/// <summary>
/// A project is a collection of files and folders that are used to create a game.
/// </summary>
public sealed class EditorProject
{
  public EditorProject(string rootPath)
  {
    RootPath = Path.GetFullPath(rootPath);
  }

  /// <summary>
  /// The root folder for the project.
  /// </summary>
  public string RootPath { get; }

  /// <summary>
  /// Source path where assets are loaded.
  /// </summary>
  public string SourcePath => Path.Combine(RootPath, "Assets");

  /// <summary>
  /// Target path where cooked assets are saved.
  /// </summary>
  public string TargetPath => Path.Combine(RootPath, "Resources");

  /// <summary>
  /// The entry point for the associated project
  /// </summary>
  public ProjectHost? Host { get; init; }
}
