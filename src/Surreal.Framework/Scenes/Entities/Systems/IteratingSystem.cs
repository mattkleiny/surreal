using Surreal.Framework.Scenes.Entities.Aspects;
using Surreal.Timing;

namespace Surreal.Framework.Scenes.Entities.Systems
{
  public abstract class IteratingSystem : SubscribedSystem
  {
    protected IteratingSystem(Aspect aspect)
      : base(aspect)
    {
    }

    public override void Input(DeltaTime deltaTime)
    {
      BeginInput(deltaTime);

      for (var i = 0; i < Entities.Length; i++)
      {
        var entityId = Entities[i];
        var entity   = World!.GetEntity(entityId);

        Input(deltaTime, entity);
      }

      EndInput(deltaTime);
    }

    public override void Update(DeltaTime deltaTime)
    {
      BeginUpdate(deltaTime);

      for (var i = 0; i < Entities.Length; i++)
      {
        var entityId = Entities[i];
        var entity   = World!.GetEntity(entityId);

        Update(deltaTime, entity);
      }

      EndUpdate(deltaTime);
    }

    public override void Draw(DeltaTime deltaTime)
    {
      BeginDraw(deltaTime);

      for (var i = 0; i < Entities.Length; i++)
      {
        var entityId = Entities[i];
        var entity   = World!.GetEntity(entityId);

        Draw(deltaTime, entity);
      }

      EndDraw(deltaTime);
    }

    protected virtual void BeginInput(DeltaTime deltaTime)
    {
    }

    protected virtual void Input(DeltaTime deltaTime, Entity entity)
    {
    }

    protected virtual void EndInput(DeltaTime deltaTime)
    {
    }

    protected virtual void BeginUpdate(DeltaTime deltaTime)
    {
    }

    protected virtual void Update(DeltaTime deltaTime, Entity entity)
    {
    }

    protected virtual void EndUpdate(DeltaTime deltaTime)
    {
    }

    protected virtual void BeginDraw(DeltaTime deltaTime)
    {
    }

    protected virtual void Draw(DeltaTime deltaTime, Entity entity)
    {
    }

    protected virtual void EndDraw(DeltaTime deltaTime)
    {
    }
  }
}
