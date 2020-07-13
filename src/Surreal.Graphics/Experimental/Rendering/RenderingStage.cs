namespace Surreal.Graphics.Experimental.Rendering {
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