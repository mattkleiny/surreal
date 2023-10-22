using Surreal.Graphics;
using Surreal.Graphics.Rendering;
using Surreal.Timing;

namespace Surreal.Scenes.Canvas;

public class SpriteNodeTests
{
  [Test]
  public void it_should_render_to_sprite_batch()
  {
    var backend = IGraphicsBackend.Headless;

    var node = new SpriteNode();

    var frame = new RenderFrame
    {
      DeltaTime = DeltaTime.OneOver60,
      Manager = new RenderContextManager(backend)
    };

    frame.Manager.AddContext(new SpriteBatchContext
    {
      Batch = new SpriteBatch(backend)
    });

    node.Render(in frame);
  }
}
