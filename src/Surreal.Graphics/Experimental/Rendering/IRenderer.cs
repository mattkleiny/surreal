using Surreal.Graphics.Cameras;

namespace Surreal.Graphics.Experimental.Rendering {
  public interface IRenderer {
    void RenderCamera(ICamera camera);
  }
}