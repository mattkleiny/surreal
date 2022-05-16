using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>Describes how to create a <see cref="RenderTarget"/>.</summary>
public readonly record struct RenderTargetDescriptor(
  int Width,
  int Height,
  TextureFormat Format,
  TextureFilterMode FilterMode = TextureFilterMode.Linear,
  TextureWrapMode WrapMode = TextureWrapMode.Clamp
);

/// <summary>Manages a frame buffer that can be rendered to.</summary>
public sealed class RenderTarget : GraphicsResource
{
  private readonly IGraphicsServer server;

  public RenderTarget(IGraphicsServer server, RenderTargetDescriptor descriptor)
  {
    this.server = server;

    ColorAttachment = new Texture(server, TextureFormat.Rgba8);
    ColorAttachment.WritePixels(descriptor.Width, descriptor.Height, ReadOnlySpan<Color32>.Empty);

    Handle = server.CreateFrameBuffer(ColorAttachment.Handle);
  }

  public Texture        ColorAttachment { get; }
  public GraphicsHandle Handle          { get; }

  public void Activate()
  {
    server.SetActiveFrameBuffer(Handle);
  }

  public void Deactivate()
  {
    server.SetActiveFrameBuffer(GraphicsHandle.None);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      ColorAttachment.Dispose();

      server.DeleteFrameBuffer(Handle);
    }

    base.Dispose(managed);
  }
}
