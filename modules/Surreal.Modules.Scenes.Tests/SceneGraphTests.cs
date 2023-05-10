using Surreal.Graphics.Rendering;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;

namespace Surreal;

public class SceneGraphTests
{
  [Test]
  public void it_should_work()
  {
  }

  private sealed class SpriteComponent : SceneComponent
  {
    public required TextureRegion TextureRegion { get; init; }

    public override void OnRender(ISceneNode node, in RenderFrame frame, IRenderContextManager manager)
    {
      base.OnRender(node, in frame, manager);

      using var scope = manager.AcquireContext<SpriteContext>(in frame);

      scope.Context.SpriteBatch.Draw(TextureRegion, Vector2.Zero);
    }
  }
}
