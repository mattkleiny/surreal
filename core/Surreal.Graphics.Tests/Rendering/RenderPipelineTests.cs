namespace Surreal.Graphics.Rendering;

public class RenderPipelineTests
{
  [Test]
  public void it_should_work()
  {
    var context = GraphicsContext.Headless;

    using var pipeline = new DeferredRenderPipeline(context);

    var cameras = new[]
    {
      Substitute.For<IRenderCamera>(),
      Substitute.For<IRenderCamera>(),
    };

    pipeline.Render(cameras);
  }
}
