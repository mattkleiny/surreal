using Surreal.Graphics.Cameras;
using Surreal.Graphics.Materials;

namespace Surreal.Graphics.Rendering.PostProcessing
{
  public readonly ref struct PostProcessingContext
  {
    public RenderingContext RenderingContext { get; }
    public FrameBuffer     SourceBuffer     { get; }
    public FrameBuffer     TargetBuffer     { get; }
    public ShaderFactory    ShaderFactory    { get; }

    public ICamera         Camera          => RenderingContext.Camera;
    public IGraphicsDevice Device          => RenderingContext.Device;
    public CommandBuffer   Commands        => RenderingContext.Commands;
    public FrameBuffer    ColorAttachment => RenderingContext.ColorAttachment;
    public FrameBuffer    DepthAttachment => RenderingContext.DepthAttachment;

    public PostProcessingContext(
      RenderingContext renderingContext,
      FrameBuffer sourceBuffer,
      FrameBuffer targetBuffer,
      ShaderFactory shaderFactory)
    {
      RenderingContext = renderingContext;
      SourceBuffer     = sourceBuffer;
      TargetBuffer     = targetBuffer;
      ShaderFactory    = shaderFactory;
    }

    public void BlitImageEffect(ShaderProgram program)
      => Commands.Blit(Device, SourceBuffer, TargetBuffer, program);

    internal PostProcessingContext SwapBuffers()
      => new PostProcessingContext(RenderingContext, TargetBuffer, SourceBuffer, ShaderFactory);
  }
}