using System;
using System.Numerics;
using Mindustry.Modules.Core.Components;
using Surreal.Collections;
using Surreal.Framework.Scenes.Entities;
using Surreal.Framework.Scenes.Entities.Aspects;
using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Framework.Scenes.Entities.Systems;
using Surreal.Graphics;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics.Linear;
using Surreal.Timing;

namespace Mindustry.Modules.Core.Systems {
  public sealed class GeometrySystem : IteratingSystem {
    private readonly GeometryBatch batch;
    private readonly ICamera       camera;

    public GeometrySystem(GeometryBatch batch, ICamera camera)
        : base(Aspect.Of<Transform>()) {
      this.batch  = batch;
      this.camera = camera;
    }

    public override void Begin() {
      base.Begin();

      batch.Begin(in camera.ProjectionView);
    }

    protected override void Draw(DeltaTime deltaTime, Entity entity) {
      base.Draw(deltaTime, entity);

      ref var transform = ref entity.Get<Transform>();
      ref var sprite    = ref entity.Get<GeometrySprite>();

      var points = new SpanList<Vector2>(stackalloc Vector2[sprite.Points.Length]);

      for (var i = 0; i < sprite.Points.Length; i++) {
        var point = sprite.Points[i];

        if (MathF.Abs(transform.Rotation) > 0f) {
          point = point.RotateByDegrees(transform.Rotation);
        }

        points.Add(transform.Position + point);
      }

      batch.DrawPrimitive(points.ToSpan(), Color.Red, sprite.Type);
    }

    public override void End() {
      batch.End();

      base.End();
    }
  }
}