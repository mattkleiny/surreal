using Surreal.Graphics.Materials;
using Surreal.Resources;

namespace Surreal.Graphics.Shaders;

/// <summary>
/// Utilities for <see cref="ShaderProgram" />s.
/// </summary>
public static class ShaderProgramExtensions
{
  public static async Task<Material> LoadDefaultWireMaterialAsync(this AssetManager manager, CancellationToken cancellationToken = default)
  {
    return await manager.LoadAssetAsync<Material>("resx://Surreal.Graphics/Resources/Embedded/shaders/wire.glsl", cancellationToken);
  }

  public static async Task<Material> LoadDefaultSpriteMaterialAsync(this AssetManager manager, CancellationToken cancellationToken = default)
  {
    return await manager.LoadAssetAsync<Material>("resx://Surreal.Graphics/Resources/Embedded/shaders/sprite.glsl", cancellationToken);
  }
}
