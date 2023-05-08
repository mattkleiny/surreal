using Surreal.Graphics.Materials;
using Surreal.Resources;

namespace Surreal.Graphics.Shaders;

/// <summary>
/// Utilities for <see cref="ShaderProgram" />s.
/// </summary>
public static class ShaderProgramExtensions
{
  public static async Task<Material> LoadDefaultWireMaterialAsync(this IResourceManager manager)
  {
    return await manager.LoadResourceAsync<Material>("resx://Surreal.Graphics/Resources/shaders/wire.glsl");
  }

  public static async Task<Material> LoadDefaultSpriteMaterialAsync(this IResourceManager manager)
  {
    return await manager.LoadResourceAsync<Material>("resx://Surreal.Graphics/Resources/shaders/sprite.glsl");
  }
}
