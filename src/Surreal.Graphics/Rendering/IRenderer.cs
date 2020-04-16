using Surreal.Graphics.Cameras;

namespace Surreal.Graphics.Rendering
{
  public interface IRenderer
  {
    void RenderCamera(ICamera camera);
  }
}