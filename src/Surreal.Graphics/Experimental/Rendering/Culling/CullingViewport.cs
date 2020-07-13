using Surreal.Mathematics;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Experimental.Rendering.Culling {
  public readonly struct CullingViewport {
    public Frustum Frustum { get; }

    public CullingViewport(Frustum frustum) {
      Frustum = frustum;
    }
  }
}