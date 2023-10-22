﻿using Surreal.Assets;

namespace Surreal.Graphics.Materials;

/// <summary>
/// Utilities for <see cref="ShaderProgram" />s.
/// </summary>
public static class ShaderProgramExtensions
{
  /// <summary>
  /// Loads a default wire material.
  /// </summary>
  public static async Task<Material> LoadDefaultWireMaterialAsync(this AssetManager manager, CancellationToken cancellationToken = default)
  {
    return await manager.LoadAssetAsync<Material>("resx://Surreal.Graphics/Resources/Embedded/shaders/wire.glsl", cancellationToken);
  }

  /// <summary>
  /// Loads a default sprite material.
  /// </summary>
  public static async Task<Material> LoadDefaultSpriteMaterialAsync(this AssetManager manager, CancellationToken cancellationToken = default)
  {
    return await manager.LoadAssetAsync<Material>("resx://Surreal.Graphics/Resources/Embedded/shaders/sprite.glsl", cancellationToken);
  }
}
