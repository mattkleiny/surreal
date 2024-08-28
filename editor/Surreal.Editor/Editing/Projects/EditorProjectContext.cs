using Surreal.Assets;
using Surreal.Editing.Workloads;
using Surreal.Services;

namespace Surreal.Editing.Projects;

/// <summary>
/// Context for an <see cref="EditorProject"/>.
/// </summary>
public sealed class EditorProjectContext : IDisposable
{
  /// <summary>
  /// Context for an <see cref="EditorProject"/>.
  /// </summary>
  public EditorProjectContext(EditorConfiguration configuration, EditorProject project, IServiceProvider services)
  {
    Configuration = configuration;
    Workloads = new EditorWorkloadContext(configuration);
    Project = project;

    Assets = new AssetDatabase(project.SourcePath, project.TargetPath)
    {
      Importers = services.GetServices<IAssetImporter>().ToList(),
      WatchForChanges = true
    };

    // import assets in the background
    Task.Run(() => Assets.ImportAssetsAsync(writeMetadataToDisk: true));
  }

  /// <summary>
  /// The configuration for the editor.
  /// </summary>
  public EditorConfiguration Configuration { get; }

  /// <summary>
  /// The project itself.
  /// </summary>
  public EditorProject Project { get; }

  /// <summary>
  /// The <see cref="AssetDatabase"/> for the project.
  /// </summary>
  public AssetDatabase Assets { get; }

  /// <summary>
  /// The workload context for the project.
  /// </summary>
  public EditorWorkloadContext Workloads { get; set; }

  public void Dispose()
  {
    Assets.Dispose();
  }
}
