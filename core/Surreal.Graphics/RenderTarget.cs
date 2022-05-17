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
  private readonly IGraphicsServer server;

  public RenderTarget(IGraphicsServer server, RenderTargetDescriptor colorDescriptor)
  {
    this.server = server;

    ColorAttachment = new Texture(server, colorDescriptor.Format);
    ColorAttachment.WritePixels(colorDescriptor.Width, colorDescriptor.Height, ReadOnlySpan<Color32>.Empty);

    Handle = server.CreateFrameBuffer(ColorAttachment.Handle);
  }

  public GraphicsHandle Handle          { get; }
  public Texture        ColorAttachment { get; }

  /// <summary>Activates the <see cref="RenderTarget"/> for the duration of the given scope.</summary>
  public RenderTargetScope Rent()
  {
    return new RenderTargetScope(this);
  }

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
    }

    base.Dispose(managed);
  }

  /// <summary>A scope for enabling a particular <see cref="RenderTarget"/>.</summary>
  public readonly struct RenderTargetScope : IDisposable
  {
    private readonly RenderTarget target;

    public RenderTargetScope(RenderTarget target)
    {
      this.target = target;

      target.Activate();
    }

    public void Dispose()
    {
      target.Deactivate();
    }
  }
}
