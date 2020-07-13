using System.Numerics;
using Surreal.Framework.Scenes.Entities.Aspects;
using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Graphics;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Sprites;
using Surreal.Mathematics;
using Surreal.Mathematics.Timing;

namespace Surreal.Framework.Scenes.Entities.Systems {
  public sealed class SpriteSystem : IteratingSystem {
    private readonly SpriteBatch batch;
    private readonly ICamera     camera;

    private IComponentMapper<Transform>? transforms;
    private IComponentMapper<Sprite>?    sprites;

    public SpriteSystem(SpriteBatch batch, ICamera camera)
        : base(Aspect.Of<Transform, Sprite>()) {
      this.batch  = batch;
      this.camera = camera;
    }

    public bool ClearTintAfterRender { get; set; } = true;

    public override void Initialize(EntityScene scene) {
      base.Initialize(scene);

      transforms = scene.GetComponentMapper<Transform>();
      sprites    = scene.GetComponentMapper<Sprite>();
    }

    protected override void BeginDraw(DeltaTime deltaTime) {
      batch.Begin(in camera.ProjectionView);
    }

    protected override void Draw(DeltaTime deltaTime, Entity entity) {
      var transform = transforms!.Get(entity.Id);
      var sprite    = sprites!.Get(entity.Id);

      if (sprite.Texture == null) return; // no texture? no worries!

      batch.Color = sprite.Tint;

      batch.DrawPivoted(
          region: sprite.Texture,
          position: transform.Position,
          rotation: transform.Rotation,
          pivot: Pivot.Center,
          scale: sprite.Scale
      );
    }

    protected override void EndDraw(DeltaTime deltaTime) {
      batch.End();

      if (ClearTintAfterRender) {
        batch.Color = Color.White;
      }
    }
  }
}