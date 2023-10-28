using Surreal.Editing.Assets;
using Surreal.Editing.Assets.Importers;

namespace Surreal.Editing.Projects;

/// <summary>
/// A project is a collection of files and folders that are used to create a game.
/// </summary>
public sealed class EditorProject
{
  public EditorProject(string rootPath, string projectFilePath)
  {
    RootPath = Path.GetFullPath(rootPath);
    ProjectFile = ProjectFile.Load(projectFilePath);

    Assets = new AssetDatabase(SourcePath, TargetPath)
    {
      Importers =
      {
        new AudioClipImporter(),
        new ColorPaletteImporter(),
        new TextureImporter()
      },
      // N.B: place this at the end; order is important!
      WatchForChanges = true
    };

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
  public ProjectHost? ProjectHost { get; init; }
}
