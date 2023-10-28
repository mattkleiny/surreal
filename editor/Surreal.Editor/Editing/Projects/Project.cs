using Surreal.Editing.Assets;
using Surreal.Editing.Assets.Importers;

namespace Surreal.Editing.Projects;

/// <summary>
/// A project is a collection of files and folders that are used to create a game.
/// </summary>
public sealed class Project
{
  /// <summary>
  /// Loads a project from the given path.
  /// </summary>
  public static Project Load(string rootPath, string projectPath)
  {
    return new Project(rootPath, ProjectFile.Load(projectPath));
  }

  private Project(string rootPath, ProjectFile projectFile)
  {
    ProjectFile = projectFile;
    RootPath = Path.GetFullPath(rootPath);
    Assets = new AssetDatabase(SourcePath, TargetPath)
    {
      Importers =
      {
        new AudioClipImporter(),
        new ColorPaletteImporter(),
        new TextureImporter()
      },
      WatchForChanges = true
    };

    // import assets in the background
    Task.Run(() => Assets.ImportAssetsAsync(writeMetadataToDisk: true));
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
  /// The primary <see cref="AssetDatabase"/> for this project.
  /// </summary>
  public AssetDatabase Assets { get; }

  /// <summary>
  /// Provides metadata about the MSBuild project.
  /// </summary>
  internal ProjectFile ProjectFile { get; }

  /// <summary>
  /// The entry point for the associated project
  /// </summary>
  public ProjectEntryPoint? EntryPoint { get; init; }
}
