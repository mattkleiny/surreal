using Surreal.Colors;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Depth bits for a <see cref="RenderTarget"/>s.
/// </summary>
public enum DepthStencilFormat : byte
{
  None,
  Depth24,
  Depth24Stencil8,
  Depth32Stencil8,
}

/// <summary>
/// Descriptor for a <see cref="RenderTarget"/>.
/// </summary>
public sealed record RenderTargetDescriptor
{
  /// <summary>
  /// The width of the render target, in pixels.
  /// </summary>
  public required uint Width { get; set; }

  /// <summary>
  /// The height of the render target, in pixels.
  /// </summary>
  public required uint Height { get; set; }

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
  public required DepthStencilFormat DepthStencilFormat { get; init; }
}

/// <summary>
/// A render target is a texture that can be rendered to.
/// </summary>
public sealed class RenderTarget(IGraphicsBackend backend, RenderTargetDescriptor descriptor) : GraphicsAsset
{
  /// <summary>
  /// The handle to the underlying render target.
  /// </summary>
  public FrameBufferHandle Handle { get; } = backend.CreateFrameBuffer(descriptor);

  /// <summary>
  /// Is this render target currently bound to the graphics backend?
  /// </summary>
  public bool IsActive => backend.IsActiveFrameBuffer(Handle);

  /// <summary>
  /// Binds this render target to the graphics backend.
  /// </summary>
  public void Bind()
  {
    backend.BindFrameBuffer(Handle);
  }

  /// <summary>
  /// Unbinds this render target from the graphics backend.
  /// </summary>
  public void Unbind()
  {
    backend.BindFrameBuffer(FrameBufferHandle.None);
  }

  /// <summary>
  /// Clears the color buffer of this render target.
  /// </summary>
  public void ClearColorBuffer(Color black)
  {
    if (!IsActive)
    {
      throw new InvalidOperationException("Cannot clear a render target that is not bound.");
    }

    backend.ClearColorBuffer(black);
  }

  /// <summary>
  /// Clears the depth buffer of this render target.
  /// </summary>
  public void ClearDepthBuffer(float depth)
  {
    if (!IsActive)
    {
      throw new InvalidOperationException("Cannot clear a render target that is not bound.");
    }

    backend.ClearDepthBuffer(depth);
  }

  /// <summary>
  /// Clears the stencil buffer of this render target.
  /// </summary>
  public void ClearStencilBuffer(int amount)
  {
    if (!IsActive)
    {
      throw new InvalidOperationException("Cannot clear a render target that is not bound.");
    }

    backend.ClearStencilBuffer(amount);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      backend.DeleteFrameBuffer(Handle);
    }

    base.Dispose(managed);
  }
}
