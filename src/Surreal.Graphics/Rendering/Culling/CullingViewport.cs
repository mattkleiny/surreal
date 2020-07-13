using Surreal.Mathematics;

namespace Surreal.Graphics.Rendering.Culling {
  public readonly struct CullingViewport {
    public Frustum Frustum { get; }

    public CullingViewport(Frustum frustum) {
      Frustum = frustum;
    }
  }
}