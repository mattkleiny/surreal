using Surreal.Assets;
using Surreal.Timing;

namespace Surreal.Graphics.Rendering;

public class RenderContextManagerTests
{
  [Test]
  public void it_should_notify_on_frame_before_acquiring_context()
  {
    using var resources = new AssetManager();
    using var manager = new RenderContextManager();

    var frame = new RenderFrame
    {
      DeltaTime = DeltaTime.Default,
      TotalTime = DeltaTime.One,
      Device = IGraphicsDevice.Null,
      Contexts = manager,
      Scene = Substitute.For<IRenderScene>(),
      Viewport = new Viewport(0, 0, 640, 480)
    };

    var context = Substitute.For<RenderContext>();
    manager.Add(context);

    manager.OnBeginFrame(in frame);
    context.Received().OnBeginFrame(frame);

    manager.OnEndFrame(in frame);
    context.Received().OnEndFrame(frame);
  }
}
