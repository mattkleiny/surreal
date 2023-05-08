using Surreal.Assets;
using Surreal.Graphics.Materials;

namespace Surreal.Graphics.Shaders;

/// <summary>
/// Utilities for <see cref="ShaderProgram" />s.
/// </summary>
public static class ShaderProgramExtensions
{
  public static async Task<Material> LoadDefaultWireMaterialAsync(this IAssetManager manager)
  {
    return await manager.LoadAssetAsync<Material>("resx://Surreal.Graphics/Resources/shaders/wire.glsl");
  }

  public static async Task<Material> LoadDefaultSpriteMaterialAsync(this IAssetManager manager)
  {
    return await manager.LoadAssetAsync<Material>("resx://Surreal.Graphics/Resources/shaders/sprite.glsl");
  }
}
