namespace Surreal.Graphics.Rendering.Culling {
  public readonly struct CulledRenderer {
    public readonly byte Layer;

    public CulledRenderer(byte layer) {
      Layer = layer;
    }
  }
}