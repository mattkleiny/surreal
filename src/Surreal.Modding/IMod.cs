using System;
using Surreal.Mathematics.Timing;

namespace Surreal {
  public interface IMod : IDisposable {
    void Initialize(IModRegistry registry);
    void Input(DeltaTime deltaTime);
    void Update(DeltaTime deltaTime);
    void Draw(DeltaTime deltaTime);
  }
}