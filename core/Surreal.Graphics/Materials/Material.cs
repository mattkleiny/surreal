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
public readonly record struct BlendState(BlendMode Source, BlendMode Target)
{
  /// <summary>
  /// A default blend state that uses <see cref="BlendMode.OneMinusSourceAlpha" />.
  /// </summary>
  public static BlendState OneMinusSourceAlpha { get; } = new(BlendMode.SourceAlpha, BlendMode.OneMinusSourceAlpha);
}

/// <summary>
/// A property of a <see cref="Material" />.
/// </summary>
[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly record struct MaterialProperty<T>(string Name);

/// <summary>
/// A set of <see cref="MaterialProperty{T}" />s in a <see cref="Material" />.
/// </summary>
[DebuggerDisplay("MaterialPropertySet ({_uniforms.Count} uniforms)")]
public sealed class MaterialPropertySet
{
  private readonly Dictionary<string, Variant> _uniforms = new();

  /// <summary>
  /// Attempts to get a uniform property from the material.
  /// </summary>
  public bool TryGetUniform<T>(MaterialProperty<T> property, out T value)
  {
    if (_uniforms.TryGetValue(property.Name, out var variant))
    {
      value = variant.As<T>();
      return true;
    }

    value = default!;
    return false;
  }

  /// <summary>
  /// Sets a uniform property on the material.
  /// </summary>
  public void SetUniform<T>(MaterialProperty<T> property, T value)
  {
    _uniforms[property.Name] = Variant.From(value);
  }
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
  /// The desired <see cref="Materials.BlendState" /> for this material.
  /// </summary>
  public BlendState? BlendState { get; set; }

  /// <summary>
  /// The properties of the material.
  /// </summary>
  public MaterialPropertySet Properties { get; } = new();

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
    backend.SetBlendState(BlendState);
  }

  /// <summary>
  /// Draws a fullscreen quad with the material.
  /// </summary>
  public void DrawFullscreenQuad()
  {
    throw new NotImplementedException();
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
