using Surreal.Graphics.Textures;

namespace Surreal.Graphics;

/// <summary>
/// Describes how to create a <see cref="RenderTarget" />.
/// </summary>
public readonly record struct RenderTargetDescriptor()
{
  /// <summary>
  /// A default <see cref="RenderTargetDescriptor" /> for standard purposes at 1080p.
  /// </summary>
  public static RenderTargetDescriptor Default { get; } = new()
  {
    Width = 1920,
    Height = 1080,
    Format = TextureFormat.Rgba8,
    FilterMode = TextureFilterMode.Point,
    WrapMode = TextureWrapMode.Clamp
  };

  public required int Width { get; init; }
  public required int Height { get; init; }
  public TextureFormat Format { get; init; } = TextureFormat.Rgba8;
  public TextureFilterMode FilterMode { get; init; } = TextureFilterMode.Linear;
  public TextureWrapMode WrapMode { get; init; } = TextureWrapMode.Clamp;
}
