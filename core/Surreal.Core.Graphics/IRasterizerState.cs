namespace Surreal.Graphics {
  public interface IRasterizerState {
    Viewport Viewport { get; set; }

    bool IsDepthTestingEnabled { get; set; }
    bool IsBlendingEnabled     { get; set; }
  }
}