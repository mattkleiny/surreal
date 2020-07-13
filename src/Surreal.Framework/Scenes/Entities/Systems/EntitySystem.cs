using Surreal.Mathematics.Timing;

namespace Surreal.Framework.Scenes.Entities.Systems {
  public abstract class EntitySystem : IEntitySystem {
    protected EntityScene? World { get; private set; }

    public virtual void Initialize(EntityScene scene) {
      World = scene;
    }

    public virtual void Input(DeltaTime deltaTime) {
    }

    public virtual void Update(DeltaTime deltaTime) {
    }

    public virtual void Draw(DeltaTime deltaTime) {
    }

    public virtual void Dispose() {
    }
  }
}