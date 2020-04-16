using System;
using System.Collections.Generic;
using Surreal.Graphics.Cameras;

namespace Surreal.Graphics.Rendering
{
  public interface IRenderingPipeline : IDisposable
  {
    void Render(ICamera camera);
    void Render(IReadOnlyList<ICamera> cameras);
  }
}