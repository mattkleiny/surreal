namespace Surreal.Graphics.Materials;

/// <summary>
/// Different kinds of blends.
/// </summary>
public enum BlendMode
{
  SourceColor,
  TargetColor,
  SourceAlpha,
  TargetAlpha,
  OneMinusSourceColor,
  OneMinusTargetColor,
  OneMinusSourceAlpha,
  OneMinusTargetAlpha
}

/// <summary>
/// State blending operation.
/// </summary>
public readonly record struct BlendState(bool IsEnabled, BlendMode Source, BlendMode Target)
{
  /// <summary>
  /// A disabled blend state.
  /// </summary>
  public static BlendState Disabled { get; } = default(BlendState) with { IsEnabled = false };

  /// <summary>
  /// A default blend state that uses <see cref="BlendMode.OneMinusSourceAlpha" />.
  /// </summary>
  public static BlendState OneMinusSourceAlpha { get; } = new(true, BlendMode.SourceAlpha, BlendMode.OneMinusSourceAlpha);
}

/// <summary>
/// A material is a configuration of the graphics state and properties used for rendering.
/// </summary>
public sealed class Material(ShaderProgram shader, bool ownsShader = true) : GraphicsAsset
{
  /// <summary>
  /// The associated <see cref="ShaderProgram" /> for the material.
  /// </summary>
  public ShaderProgram Shader { get; } = shader;

  /// <summary>
  /// The desired <see cref="BlendState" /> for this material.
  /// </summary>
  public BlendState Blending { get; set; } = BlendState.OneMinusSourceAlpha;

  /// <summary>
  /// Applies the material properties to the underlying shader.
  /// </summary>
  public void Apply(IGraphicsBackend backend)
  {
    // bind the shader
    backend.SetActiveShader(Shader.Handle);

    // apply locals
    // ApplyUniforms(Properties.Uniforms);
    // ApplySamplers(Properties.Samplers);

    // apply blend state
    backend.SetBlendState(Blending);
  }

  protected override void Dispose(bool managed)
  {
    if (managed && ownsShader)
    {
      Shader.Dispose();
    }

    base.Dispose(managed);
  }
}
