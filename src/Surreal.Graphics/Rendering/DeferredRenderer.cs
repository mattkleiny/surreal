using Surreal.Graphics.Rendering.Culling;

namespace Surreal.Graphics.Rendering
{
  public class DeferredRenderer : Renderer
  {
    public DeferredRenderer(
      IGraphicsDevice device,
      ICullingStrategy cullingStrategy,
      in FrameBufferDescriptor colorDescriptor,
      in FrameBufferDescriptor depthDescriptor
    ) : base(device, cullingStrategy, colorDescriptor, depthDescriptor)
    {
    }
  }
}