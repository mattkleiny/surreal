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
    registry.AddService<IAssetImporter, AudioClipImporter>();
    registry.AddService<IAssetImporter, ColorPaletteImporter>();
    registry.AddService<IAssetImporter, TextureImporter>();
  }
}
