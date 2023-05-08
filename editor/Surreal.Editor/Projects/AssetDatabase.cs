namespace Surreal.Editor.Projects;

/// <summary>
/// An asset database is a collection of files and folders that are used to create a game.
/// </summary>
public class AssetDatabase
{
  public AssetDatabase(string sourcePath, string targetPath)
  {
    SourcePath = sourcePath;
    TargetPath = targetPath;
  }

  public string SourcePath { get; }
  public string TargetPath { get; }
}
