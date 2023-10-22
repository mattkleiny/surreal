using Surreal.Resources;
using Surreal.Timing;

namespace Surreal.Graphics.Rendering;

public class RenderContextManagerTests
{
  [Test]
  public void it_should_notify_on_frame_before_acquiring_context()
  {
    using var resources = new AssetManager();
    using var manager = new RenderContextManager(IGraphicsBackend.Headless);

    var context = Substitute.For<IRenderContext>();
    var frame = new RenderFrame
    {
      DeltaTime = DeltaTime.OneOver60,
      Manager = manager
    };

    manager.AddContext(context);

    manager.OnBeginFrame(in frame);
    context.Received().OnBeginFrame(frame);

    manager.OnEndFrame(in frame);
    context.Received().OnEndFrame(frame);
  }
}
