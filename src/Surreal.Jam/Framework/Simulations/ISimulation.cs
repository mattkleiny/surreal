using System;
using Surreal.Timing;

namespace Surreal.Framework.Simulations {
  public interface ISimulation : IDisposable {
    void Initialize();
    void Begin();
    void Input(DeltaTime deltaTime);
    void Update(DeltaTime deltaTime);
    void Draw(DeltaTime deltaTime);
    void End();
  }
}