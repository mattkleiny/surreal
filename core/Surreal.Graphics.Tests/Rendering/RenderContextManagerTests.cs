﻿using Surreal.Resources;
using Surreal.Timing;

namespace Surreal.Graphics.Rendering;

public class RenderContextManagerTests
{
  [Test]
  public void it_should_notify_on_use_when_acquiring_render_context()
  {
    using var resources = new ResourceManager();
    using var manager = new RenderContextManager(GraphicsContext.Headless, resources);

    var context = Substitute.For<IRenderContext>();
    var frame = new RenderFrame { DeltaTime = DeltaTime.OneOver60 };

    manager.AddContext(context);

    using (var _ = manager.AcquireContext<IRenderContext>(in frame))
    {
      context.Received().OnBeginUse(frame);
    }

    context.Received().OnEndUse(frame);
  }

  [Test]
  public void it_should_notify_on_frame_before_acquiring_context()
  {
    var graphics = GraphicsContext.Headless;

    using var resources = new ResourceManager();
    using var manager = new RenderContextManager(graphics, resources);

    var context = Substitute.For<IRenderContext>();
    var frame = new RenderFrame { DeltaTime = DeltaTime.OneOver60 };

    manager.AddContext(context);

    manager.OnBeginFrame(in frame);
    context.Received().OnBeginFrame(frame);

    manager.OnEndFrame(in frame);
    context.Received().OnEndFrame(frame);
  }
}
