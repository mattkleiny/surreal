using Surreal.Timing;

namespace Surreal.Graphics.Cameras;

/// <summary>Provides control of a camera.</summary>
public interface ICameraController
{
  void Input(DeltaTime deltaTime);
}
