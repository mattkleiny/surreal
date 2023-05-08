namespace Surreal.Editor.Projects;

/// <summary>
/// A project is a collection of files and folders that are used to create a game.
/// </summary>
public sealed class Project
{
  /// <summary>
  /// A project is a collection of files and folders that are used to create a game.
  /// </summary>
  public Project(string sourcePath, string targetPath)
  {
    SourcePath = sourcePath;
    TargetPath = targetPath;

    Assets = new AssetDatabase(sourcePath, targetPath);
  }

  public string SourcePath { get; init; }
  public string TargetPath { get; }

  /// <summary>
  /// The primary asset database for this project.
  /// </summary>
  public AssetDatabase Assets { get; }
}
