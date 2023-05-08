using Surreal.Colors;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics;

/// <summary>
/// Manages a frame buffer that can be rendered to.
/// </summary>
public sealed class RenderTarget : GraphicsResource
{
  private readonly IGraphicsServer _server;

  public RenderTarget(IGraphicsServer server, RenderTargetDescriptor colorDescriptor)
  {
    _server = server;

    ColorAttachment = new Texture(server, colorDescriptor.Format, colorDescriptor.FilterMode, colorDescriptor.WrapMode);
    ColorAttachment.WritePixels(colorDescriptor.Width, colorDescriptor.Height, ReadOnlySpan<ColorB>.Empty);

    Handle = server.CreateFrameBuffer(ColorAttachment.Handle);
  }

  public GraphicsHandle Handle { get; }
  public Texture ColorAttachment { get; }

  /// <summary>
  /// Activates the <see cref="RenderTarget" /> for the duration of the given scope.
  /// </summary>
  public RenderTargetScope ActivateForScope()
  {
    return new RenderTargetScope(this);
  }

  /// <summary>
  /// Activates this as the primary render target.
  /// </summary>
  public void Activate()
  {
    _server.SetActiveFrameBuffer(Handle);
  }

  /// <summary>
  /// Deactivates this as the primary render target and swaps back to teh default.
  /// </summary>
  public void Deactivate()
  {
    _server.SetDefaultFrameBuffer();
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      _server.DeleteFrameBuffer(Handle);

      ColorAttachment.Dispose();
    }

    base.Dispose(managed);
  }

  /// <summary>
  /// A scope for enabling a particular <see cref="RenderTarget" />.
  /// </summary>
  public readonly struct RenderTargetScope : IDisposable
  {
    private readonly RenderTarget _target;

    public RenderTargetScope(RenderTarget target)
    {
      _target = target;

      target.Activate();
    }

    public void Dispose()
    {
      _target.Deactivate();
    }
  }
}
