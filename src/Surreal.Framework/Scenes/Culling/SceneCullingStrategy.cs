using Surreal.Collections;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Rendering.Culling;
using Surreal.Memory;

namespace Surreal.Framework.Scenes.Culling {
  public sealed class SceneCullingStrategy<TScene> : ICullingStrategy
      where TScene : IScene, ICullingProvider {
    private readonly IBuffer<CulledRenderer> buffer;
    private readonly TScene                  scene;

    public SceneCullingStrategy(TScene scene, int capacity = 4096) {
      this.scene = scene;

      buffer = Buffers.Allocate<CulledRenderer>(capacity);
    }

    public CullingResults PerformCulling(ICamera camera) {
      var viewport = new CullingViewport(camera.Frustum);
      var results  = new SpanList<CulledRenderer>(buffer.Span);

      scene.CullRenderers(in viewport, ref results);

      return default; // TODO: implement culling results
    }
  }
}