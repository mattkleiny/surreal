using Surreal.Assets;
using Surreal.Graphics.Textures;

namespace Surreal.Editor.Importers;

/// <summary>
/// Imports <see cref="Texture"/> data.
/// </summary>
internal sealed class TextureImporter : AssetImporter<Texture>
{
  public override bool CanHandle(string path)
  {
    return path.EndsWith(".png");
  }

  public override Task<Texture> ImportAsync(string path, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
