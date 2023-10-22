using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Depth bits for a <see cref="RenderTarget"/>s.
/// </summary>
public enum DepthBits : byte
{
  None = 0,
  Eight = 8,
  Sixteen = 16,
}

/// <summary>
/// Stencil bits for a <see cref="RenderTarget"/>s.
/// </summary>
public enum StencilBits : byte
{
  None = 0,
  Eight = 8,
  Sixteen = 16,
}

/// <summary>
/// Descriptor for a <see cref="RenderTarget"/>.
/// </summary>
public sealed record RenderTargetDescriptor
{
  /// <summary>
  /// The format of the render target.
  /// </summary>
  public required TextureFormat Format { get; init; }

  /// <summary>
  /// The wrap mode of the render target.
  /// </summary>
  public required TextureWrapMode WrapMode { get; init; }

  /// <summary>
  /// The filter mode of the render target.
  /// </summary>
  public required TextureFilterMode FilterMode { get; init; }

  /// <summary>
  /// The number of depth bits to allocate in the render target.
  /// </summary>
  public required DepthBits DepthBits { get; init; }

  /// <summary>
  /// The number of stencil bits to allocate in the render target.
  /// </summary>
  public required StencilBits Stencil { get; init; }
}

/// <summary>
/// A render target is a texture that can be rendered to.
/// </summary>
public sealed class RenderTarget(IGraphicsBackend backend, RenderTargetDescriptor descriptor) : GraphicsAsset;
