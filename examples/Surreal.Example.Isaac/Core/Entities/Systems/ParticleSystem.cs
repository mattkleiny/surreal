using System;
using Isaac.Core.Entities.Components;
using Surreal.Framework.Scenes.Entities;
using Surreal.Framework.Scenes.Entities.Aspects;
using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Framework.Scenes.Entities.Systems;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Sprites;
using Surreal.Timing;

namespace Isaac.Core.Entities.Systems
{
  public sealed class ParticleSystem : IteratingSystem
  {
    private readonly SpriteBatch batch;
    private readonly ICamera     camera;

    private IComponentMapper<ParticleEmitter>? particleEmitters;

    public ParticleSystem(SpriteBatch batch, ICamera camera)
      : base(Aspect.Of<ParticleEmitter>())
    {
      this.batch  = batch;
      this.camera = camera;
    }

    public override void Initialize(EntityScene scene)
    {
      particleEmitters = scene.GetComponentMapper<ParticleEmitter>();
    }

    protected override void Update(DeltaTime deltaTime, Entity entity)
    {
      throw new NotImplementedException();
    }

    protected override void BeginDraw(DeltaTime deltaTime)
    {
      batch.Begin(in camera.ProjectionView);
    }

    protected override void Draw(DeltaTime deltaTime, Entity entity)
    {
      throw new NotImplementedException();
    }

    protected override void EndDraw(DeltaTime deltaTime)
    {
      batch.End();
    }
  }
}