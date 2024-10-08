﻿using Surreal.Colors;
using Surreal.Graphics.Materials;
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
  Depth32Stencil8
}

/// <summary>
/// Indicates which buffers to blit when blitting a <see cref="RenderTarget"/>.
/// </summary>
[Flags]
public enum BlitMask : byte
{
  None = 0,
  Color = 1 << 0,
  Depth = 1 << 1,
  Stencil = 1 << 2
}

/// <summary>
/// Descriptor for a <see cref="RenderTarget"/>.
/// </summary>
public sealed record RenderTargetDescriptor
{
  /// <summary>
  /// The width of the render target, in pixels.
  /// </summary>
  public Optional<uint> Width { get; set; }

  /// <summary>
  /// The height of the render target, in pixels.
  /// </summary>
  public Optional<uint> Height { get; set; }

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
public sealed class RenderTarget(IGraphicsDevice device, RenderTargetDescriptor descriptor) : Disposable
{
  /// <summary>
  /// The handle to the underlying render target.
  /// </summary>
  public FrameBufferHandle Handle { get; } = device.CreateFrameBuffer(descriptor);

  /// <summary>
  /// Is this render target currently bound to the graphics device?
  /// </summary>
  public bool IsActive => device.IsActiveFrameBuffer(Handle);

  /// <summary>
  /// The width of this render target, in pixels.
  /// </summary>
  public uint Width { get; private set; } = descriptor.Width.GetOrDefault(device.GetViewportSize().Width);

  /// <summary>
  /// The height of this render target, in pixels.
  /// </summary>
  public uint Height { get; private set; } = descriptor.Height.GetOrDefault(device.GetViewportSize().Height);

  /// <summary>
  /// Binds this render target to the graphics device.
  /// </summary>
  public void BindToDisplay()
  {
    device.BindFrameBuffer(Handle);
  }

  /// <summary>
  /// Unbinds this render target from the graphics device.
  /// </summary>
  public void UnbindFromDisplay()
  {
    device.UnbindFrameBuffer();
  }

  /// <summary>
  /// Resizes this render target, if necessary.
  /// </summary>
  public void ResizeFrameBuffer(uint width, uint height)
  {
    if (Width != width || Height != height)
    {
      device.ResizeFrameBuffer(Handle, width, height);

      Width = width;
      Height = height;
    }
  }

  /// <summary>
  /// Clears the color buffer of this render target.
  /// </summary>
  public void ClearColorBuffer(Color color)
  {
    if (!IsActive)
    {
      throw new InvalidOperationException("Cannot clear a render target that is not bound.");
    }

    device.ClearColorBuffer(color);
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

    device.ClearDepthBuffer(depth);
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

    device.ClearStencilBuffer(amount);
  }

  /// <summary>
  /// Blits this render target to the main frame buffer.
  /// </summary>
  public GraphicsTask BlitToDisplayFrameBuffer(
    Material material,
    Optional<TextureFilterMode> filterMode = default,
    Optional<TextureWrapMode> wrapMode = default)
  {
    return BlitToDisplayFrameBuffer(material, ShaderProperty.Texture, filterMode, wrapMode);
  }

  /// <summary>
  /// Blits this render target to the main frame buffer.
  /// </summary>
  public GraphicsTask BlitToDisplayFrameBuffer(
    Material material,
    ShaderProperty<TextureSampler> textureProperty,
    Optional<TextureFilterMode> filterMode = default,
    Optional<TextureWrapMode> wrapMode = default)
  {
    return device.BlitToDisplayFrameBufferAsync(Handle, material, textureProperty, filterMode, wrapMode);
  }

  /// <summary>
  /// Blits this render target to another render target.
  /// </summary>
  public GraphicsTask BlitToRenderTarget(
    RenderTarget target,
    Material material,
    ShaderProperty<TextureSampler> textureProperty,
    Optional<TextureFilterMode> filterMode = default,
    Optional<TextureWrapMode> wrapMode = default)
  {
    return device.BlitToTargetFrameBufferAsync(Handle, target.Handle, material, textureProperty, filterMode, wrapMode);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      device.DeleteFrameBuffer(Handle);
    }

    base.Dispose(managed);
  }
}
