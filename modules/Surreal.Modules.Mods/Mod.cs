using Surreal.Mathematics.Timing;

namespace Surreal {
  public abstract class Mod : IMod {
    public virtual void Initialize(IModRegistry registry) {
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