using Surreal.Framework;
using Surreal.Framework.Components;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Sprites;

namespace Asteroids.Components
{
  public class SpriteSystem : IteratingSystem
  {
    private readonly SpriteBatch                           batch;
    private readonly MaterialPass                          materialPass;
    private readonly Camera                                camera;
    private readonly IComponentStorage<TransformComponent> transforms;
    private readonly IComponentStorage<SpriteComponent>    sprites;

    public SpriteSystem(IComponentContext context, SpriteBatch batch, MaterialPass materialPass, Camera camera)
        : base(context, Aspect.Of<TransformComponent, SpriteComponent>())
    {
      this.batch        = batch;
      this.materialPass = materialPass;
      this.camera       = camera;

      transforms = context.GetStorage<TransformComponent>();
      sprites    = context.GetStorage<SpriteComponent>();
    }

    protected override void OnBeginDraw(GameTime time)
    {
      batch.Begin(materialPass, in camera.ProjectionView);
    }

    protected override void OnDraw(GameTime time, ActorId actor)
    {
      ref var transform = ref transforms.GetComponent(actor);
      ref var sprite    = ref sprites.GetComponent(actor);
    }

    protected override void OnEndDraw(GameTime time)
    {
      batch.Flush();
    }
  }
}