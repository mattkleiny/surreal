namespace Surreal.Graphics.Rendering;

public class RenderPipelineTests
{
  [Test]
  public void it_should_work()
  {
    using var graphics = GraphicsServer.CreateHeadless();
    using var pipeline = new DeferredRenderPipeline(graphics);

    var cameras = new[]
    {
      Substitute.For<IRenderCamera>(),
      Substitute.For<IRenderCamera>(),
    };

    pipeline.Render(cameras);
  }
}
