using Surreal.Graphics.Textures;

namespace Surreal.Graphics;

/// <summary>
/// Describes how to create a <see cref="RenderTarget" />.
/// </summary>
public readonly record struct RenderTargetDescriptor(int Width, int Height, TextureFormat Format, TextureFilterMode FilterMode, TextureWrapMode WrapMode)
{
  /// <summary>
  /// A default <see cref="RenderTargetDescriptor" /> for standard purposes at 1080p.
  /// </summary>
  public static RenderTargetDescriptor Default { get; } = new(1920, 1080, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.Clamp);
}