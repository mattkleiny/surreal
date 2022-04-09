using Surreal.Timing;

namespace Surreal.Graphics.Cameras;

/// <summary>Provides control of a camera.</summary>
public interface IController
{
  void OnInput(DeltaTime deltaTime);
}
