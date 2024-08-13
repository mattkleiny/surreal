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
/// How to rasterize a polygon in a <see cref="Material"/>.
/// </summary>
public enum PolygonMode
{
  Lines,
  Filled
}

/// <summary>
/// Possible culling modes for a <see cref="Material"/>.
/// </summary>
public enum CullingMode
{
  Disabled,
  Front,
  Back,
  Both
}

/// <summary>
/// Blend state options
/// </summary>
public readonly record struct BlendState(BlendMode Source, BlendMode Target)
{
  /// <summary>
  /// A default blend state that uses <see cref="BlendMode.OneMinusSourceAlpha" />.
  /// </summary>
  public static BlendState OneMinusSourceAlpha { get; } = new(BlendMode.SourceAlpha, BlendMode.OneMinusSourceAlpha);
}

/// <summary>
/// Scissor state options.
/// </summary>
public readonly record struct ScissorState(int Left, int Top, int Right, int Bottom);

/// <summary>
/// A material is a configuration of the graphics state and properties used for rendering.
/// </summary>
public sealed class Material(IGraphicsBackend backend, ShaderProgram shader, bool ownsShader = true) : Disposable
{
  /// <summary>
  /// The associated <see cref="ShaderProgram" /> for the material.
  /// </summary>
  public ShaderProgram Shader => shader;

  /// <summary>
  /// The blend state for this material.
  /// </summary>
  public BlendState? BlendState { get; set; }

  /// <summary>
  /// Scissor test state for this material.
  /// </summary>
  public ScissorState? ScissorState { get; set; }

  /// <summary>
  /// The polygon mode to use for the material.
  /// </summary>
  public PolygonMode PolygonMode { get; set; } = PolygonMode.Filled;

  /// <summary>
  /// Back-face culling mode.
  /// </summary>
  public CullingMode CullingMode { get; set; } = CullingMode.Disabled;

  /// <summary>
  /// The <see cref="UniformSet"/> of the material.
  /// </summary>
  public UniformSet Uniforms { get; } = new();

  /// <summary>
  /// Applies the material properties to the underlying shader.
  /// </summary>
  public void ApplyMaterial()
  {
    // bind the shader
    backend.SetActiveShader(Shader.Handle);

    // apply shader properties
    foreach (var (name, value) in Uniforms)
    {
      shader.SetUniform(name, value);
    }

    // apply blend state
    backend.SetBlendState(BlendState);
    backend.SetScissorState(ScissorState);
    backend.SetPolygonMode(PolygonMode);
    backend.SetCullingMode(CullingMode);
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
