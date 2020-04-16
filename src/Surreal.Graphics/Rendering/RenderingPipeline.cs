using System.Collections.Generic;
using Surreal.Graphics.Cameras;

namespace Surreal.Graphics.Rendering
{
  public abstract class RenderingPipeline : IRenderingPipeline
  {
    public void Render(ICamera camera)
    {
      BeforeAll();

      BeforeCamera(camera);
      RenderCamera(camera);
      AfterCamera(camera);

      AfterAll();
    }

    public void Render(IReadOnlyList<ICamera> cameras)
    {
      BeforeAll();

      for (var i = 0; i < cameras.Count; i++)
      {
        var camera = cameras[i];

        BeforeCamera(camera);
        RenderCamera(camera);
        AfterCamera(camera);
      }

      AfterAll();
    }

    protected virtual void BeforeAll()
    {
    }

    protected virtual void BeforeCamera(ICamera camera)
    {
    }

    protected virtual void RenderCamera(ICamera camera)
    {
    }

    protected virtual void AfterCamera(ICamera camera)
    {
    }

    protected virtual void AfterAll()
    {
    }

    public virtual void Dispose()
    {
    }
  }
}
