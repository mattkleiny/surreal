using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>Describes how to create a <see cref="RenderTarget"/>.</summary>
public readonly record struct RenderTargetDescriptor(int Width, int Height, TextureFormat Format, TextureFilterMode FilterMode, TextureWrapMode WrapMode)
{
  /// <summary>A default <see cref="RenderTargetDescriptor"/> for standard purposes at 1080p.</summary>
  public static RenderTargetDescriptor Default { get; } = new(1920, 1080, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.Clamp);
}

/// <summary>Manages a frame buffer that can be rendered to.</summary>
public sealed class RenderTarget : GraphicsResource
{
  // TODO: some more work on this?

  private readonly IGraphicsServer server;

  public RenderTarget(
    IGraphicsServer server,
    RenderTargetDescriptor colorDescriptor,
    RenderTargetDescriptor? depthDescriptor = null,
    RenderTargetDescriptor? stencilDescriptor = null)
  {
    this.server = server;

    ColorAttachment = new Texture(server, colorDescriptor.Format);
    ColorAttachment.WritePixels(colorDescriptor.Width, colorDescriptor.Height, ReadOnlySpan<Color32>.Empty);

    if (depthDescriptor != null)
    {
      DepthAttachment = new Texture(server, depthDescriptor.Value.Format);
      DepthAttachment.WritePixels(depthDescriptor.Value.Width, depthDescriptor.Value.Height, ReadOnlySpan<Color32>.Empty);
    }

    if (stencilDescriptor != null)
    {
      StencilAttachment = new Texture(server, stencilDescriptor.Value.Format);
      StencilAttachment.WritePixels(stencilDescriptor.Value.Width, stencilDescriptor.Value.Height, ReadOnlySpan<Color32>.Empty);
    }

    Handle = server.CreateFrameBuffer(ColorAttachment.Handle, DepthAttachment?.Handle, StencilAttachment?.Handle);
  }

  public GraphicsHandle Handle { get; }

  public Texture  ColorAttachment   { get; }
  public Texture? DepthAttachment   { get; }
  public Texture? StencilAttachment { get; }

  /// <summary>Activates this as the primary render target.</summary>
  public void Activate()
  {
    server.SetActiveFrameBuffer(Handle);
  }

  /// <summary>Deactivates this as the primary render target and swaps back to teh default.</summary>
  public void Deactivate()
  {
    server.SetDefaultFrameBuffer();
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteFrameBuffer(Handle);

      ColorAttachment.Dispose();
      DepthAttachment?.Dispose();
      StencilAttachment?.Dispose();
    }

    base.Dispose(managed);
  }
}
