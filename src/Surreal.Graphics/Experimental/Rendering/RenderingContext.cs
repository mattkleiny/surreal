using Surreal.Graphics.Cameras;
using Surreal.Graphics.Experimental.Rendering.Culling;

namespace Surreal.Graphics.Experimental.Rendering {
  public readonly ref struct RenderingContext {
    public IGraphicsDevice Device          { get; }
    public ICamera         Camera          { get; }
    public CullingResults  CullingResults  { get; }
    public CommandBuffer   Commands        { get; }
    public FrameBuffer     ColorAttachment { get; }
    public FrameBuffer     DepthAttachment { get; }

    public RenderingContext(
        IGraphicsDevice device,
        ICamera camera,
        CommandBuffer commands,
        CullingResults cullingResults,
        FrameBuffer colorAttachment,
        FrameBuffer depthAttachment) {
      Device          = device;
      Camera          = camera;
      Commands        = commands;
      CullingResults  = cullingResults;
      ColorAttachment = colorAttachment;
      DepthAttachment = depthAttachment;
    }
  }
}