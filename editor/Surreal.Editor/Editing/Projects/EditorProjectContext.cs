using Surreal.Editing.Assets;
using Surreal.Utilities;

namespace Surreal.Editing.Projects;

/// <summary>
/// Context for an <see cref="EditorProject"/>.
/// </summary>
public sealed class EditorProjectContext(EditorConfiguration configuration, EditorProject project, IServiceProvider services) : IDisposable
{
  /// <summary>
  /// The configuration for the editor.
  /// </summary>
  public EditorConfiguration Configuration => configuration;

  /// <summary>
  /// The project itself.
  /// </summary>
  public EditorProject Project => project;

  /// <summary>
  /// The <see cref="AssetDatabase"/> for the project.
  /// </summary>
  public AssetDatabase Assets { get; } = new(project.SourcePath, project.TargetPath)
  {
    Importers = services.GetServices<IAssetImporter>().ToList()
  };

  public void Dispose()
  {
    Assets.Dispose();
  }
}
