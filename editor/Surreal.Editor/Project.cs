using Surreal.Assets;
using Surreal.Editor.Importers;

namespace Surreal.Editor;

/// <summary>
/// A project is a collection of files and folders that are used to create a game.
/// </summary>
public sealed class Project
{
  /// <summary>
  /// A project is a collection of files and folders that are used to create a game.
  /// </summary>
  public Project(string rootPath)
  {
    RootPath = rootPath;
    SourcePath = Path.GetFullPath(Path.Combine(rootPath, "Assets"));
    TargetPath = Path.GetFullPath(Path.Combine(rootPath, "Resources"));

    Assets = new AssetDatabase(SourcePath, TargetPath)
    {
      Importers =
      {
        new BitmapFontImporter(),
        new GlslShaderProgramImporter(),
        new ImageImporter(),
        new TextureImporter()
      }
    };

    Assets.RefreshAsync(); // don't await; initial refresh
  }

  public string RootPath { get; }
  public string SourcePath { get; }
  public string TargetPath { get; }

  /// <summary>
  /// The primary <see cref="AssetDatabase"/> for this project.
  /// </summary>
  public AssetDatabase Assets { get; }
}
