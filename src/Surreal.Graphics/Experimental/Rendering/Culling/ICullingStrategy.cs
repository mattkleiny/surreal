using Surreal.Graphics.Cameras;

namespace Surreal.Graphics.Experimental.Rendering.Culling {
  public interface ICullingStrategy {
    CullingResults PerformCulling(ICamera camera);
  }
}