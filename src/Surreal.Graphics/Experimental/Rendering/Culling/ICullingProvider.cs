using Surreal.Collections;

namespace Surreal.Graphics.Experimental.Rendering.Culling {
  public interface ICullingProvider {
    void CullRenderers(in CullingViewport viewport, ref SpanList<CulledRenderer> results);
  }
}