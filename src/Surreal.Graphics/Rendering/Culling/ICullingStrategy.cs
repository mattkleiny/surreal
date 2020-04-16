using Surreal.Graphics.Cameras;

namespace Surreal.Graphics.Rendering.Culling
{
  public interface ICullingStrategy
  {
    CullingResults PerformCulling(ICamera camera);
  }
}