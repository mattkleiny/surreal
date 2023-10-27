namespace Surreal.Editing.Projects;

/// <summary>
/// An asset database is a collection of files and folders that are used to create a game.
/// </summary>
public class AssetDatabase(string sourcePath, string targetPath)
{
  public string SourcePath { get; } = sourcePath;
  public string TargetPath { get; } = targetPath;

  /// <summary>
  /// Refreshes the asset database.
  /// </summary>
  public Task RefreshAsync()
  {
    return Task.CompletedTask;
  }
}
