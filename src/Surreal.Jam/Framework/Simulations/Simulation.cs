using Surreal.Timing;

namespace Surreal.Framework.Simulations {
  public class Simulation : ISimulation {
    public virtual void Initialize() {
    }

    public virtual void Begin() {
    }

    public virtual void Input(DeltaTime deltaTime) {
    }

    public virtual void Update(DeltaTime deltaTime) {
    }

    public virtual void Draw(DeltaTime deltaTime) {
    }

    public virtual void End() {
    }

    public virtual void Dispose() {
    }
  }
}