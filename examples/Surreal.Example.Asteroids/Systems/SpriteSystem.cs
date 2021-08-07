using Asteroids.Components;
using Surreal.Framework;
using Surreal.Framework.Actors;
using Surreal.Framework.Actors.Components;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Sprites;

namespace Asteroids.Systems
{
  public sealed class SpriteSystem : IteratingSystem
  {
    private readonly SpriteBatch                           batch;
    private readonly Camera                                camera;
    private readonly Material                              material;
    private readonly IComponentStorage<SpriteComponent>    sprites;
    private readonly IComponentStorage<TransformComponent> transforms;

    public SpriteSystem(IComponentContext context, SpriteBatch batch, Material material, Camera camera)
        : base(context, Aspect.Of<TransformComponent, SpriteComponent>())
    {
      this.batch    = batch;
      this.material = material;
      this.camera   = camera;

      transforms = context.GetStorage<TransformComponent>();
      sprites    = context.GetStorage<SpriteComponent>();
    }

    protected override void OnBeginDraw(GameTime time)
    {
      batch.Begin(material[0], in camera.ProjectionView);
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
