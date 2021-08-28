using Surreal.Timing;

namespace Surreal
{
  public interface IFrameListener
  {
    void Tick(DeltaTime deltaTime);
  }
}
