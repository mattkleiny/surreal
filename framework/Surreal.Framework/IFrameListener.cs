using Surreal.Mathematics.Timing;

namespace Surreal.Framework {
  public interface IFrameListener {
    void Tick(DeltaTime deltaTime);
  }
}