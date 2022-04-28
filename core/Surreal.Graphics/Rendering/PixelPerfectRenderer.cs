using Surreal.Graphics.Shaders;

namespace Surreal.Graphics.Rendering;

/// <summary>Modern pixel perfect graphics with variable configuration and settings.</summary>
public sealed class PixelPerfectRenderer : PassBasedRenderer
{
  public PixelPerfectRenderer()
  {
    Passes.Add(new ForwardPass()); // we always require a forward pass

    IsMotionVectorsEnabled  = true;
    IsVolumetricsEnabled    = true;
    IsPostProcessingEnabled = true;
  }

  public bool IsMotionVectorsEnabled
  {
    get => Passes.Contains<MotionPass>();
    set => Passes.TogglePass<MotionPass>(value);
  }

  public bool IsVolumetricsEnabled
  {
    get => Passes.Contains<VolumetricsPass>();
    set => Passes.TogglePass<VolumetricsPass>(value);
  }

  public bool IsPostProcessingEnabled
  {
    get => Passes.Contains<PostProcessingPass>();
    set => Passes.TogglePass<PostProcessingPass>(value);
  }

  private sealed class ForwardPass : RenderPass
  {
    private ShaderProgram? shader;

    protected override async Task InitializeAsync(GraphicsContext context)
    {
      shader = await context.Assets.LoadAsset<ShaderProgram>("resx://Surreal.Graphics/Resources/shaders/sprite/forward.shade");
    }
  }

  private sealed class MotionPass : RenderPass
  {
  }

  private sealed class VolumetricsPass : RenderPass
  {
  }

  private sealed class PostProcessingPass : RenderPass
  {
  }
}
