using Surreal.Assets;
using Surreal.Timing;

namespace Surreal.Graphics.Rendering;

public class RenderContextManagerTests
{
  [Test]
  public void it_should_notify_on_frame_before_acquiring_context()
  {
    var backend = IGraphicsBackend.Headless;

    using var resources = new AssetManager();
    using var manager = new RenderContextManager(backend);

    var context = Substitute.For<IRenderContext>();
    var frame = new RenderFrame
    {
      DeltaTime = DeltaTime.OneOver60,
      Backend = backend,
      Contexts = manager,
      Scene = Substitute.For<IRenderScene>(),
      Viewport = new Viewport(0, 0, 640, 480)
    };

    manager.Add(context);

    manager.OnBeginFrame(in frame);
    context.Received().OnBeginFrame(frame);

    manager.OnEndFrame(in frame);
    context.Received().OnEndFrame(frame);
  }
}
