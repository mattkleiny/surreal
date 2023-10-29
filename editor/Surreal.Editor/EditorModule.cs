using Surreal.Editing.Assets;
using Surreal.Editing.Assets.Importers;
using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// A <see cref="IServiceModule"/> for the editor.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class EditorModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddService<IAssetImporter>(new AudioClipImporter());
    registry.AddService<IAssetImporter>(new ColorPaletteImporter());
    registry.AddService<IAssetImporter>(new TextureImporter());
  }
}
