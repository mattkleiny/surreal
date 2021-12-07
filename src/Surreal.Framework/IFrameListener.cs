using Surreal.Timing;

namespace Surreal;

/// <summary>Allows a component to listen to engine tick events.</summary>
public interface IFrameListener
{
  void Tick(DeltaTime deltaTime);
}