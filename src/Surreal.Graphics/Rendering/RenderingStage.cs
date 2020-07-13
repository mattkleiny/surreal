namespace Surreal.Graphics.Rendering {
  public enum RenderingStage {
    BeforeAll,
    BeforeOpaque,
    AfterOpaque,
    BeforeTransparent,
    AfterTransparent,
    BeforePostProcessing,
    AfterPostProcessing,
    AfterAll
  }
}