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
/// A property of a <see cref="Material" />.
/// </summary>
[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly record struct MaterialProperty<T>(string Name)
{
  public static implicit operator MaterialProperty<T>(string value) => new(value);
}

/// <summary>
/// Indicates that a <see cref="MaterialProperty{T}"/> does not exist on a material.
/// </summary>
public sealed class InvalidMaterialPropertyException(string? message = "An invalid material property was specified") : ApplicationException(message);

/// <summary>
/// A set of <see cref="MaterialProperty{T}" />s in a <see cref="Material" />.
/// </summary>
[DebuggerDisplay("MaterialPropertySet ({_uniforms.Count} uniforms)")]
public sealed class MaterialPropertySet : IEnumerable<KeyValuePair<string, Variant>>
{
  private readonly Dictionary<string, Variant> _uniforms = new();

  /// <summary>
  /// Adds a uniform property to the material.
  /// </summary>
  public void Add<T>(MaterialProperty<T> property, T value)
  {
    _uniforms.Add(property.Name, Variant.From(value));
  }

  /// <summary>
  /// Removes a uniform property from the material.
  /// </summary>
  public void Remove<T>(MaterialProperty<T> property)
  {
    _uniforms.Remove(property.Name);
  }

  /// <summary>
  /// Determines if the material contains the given property.
  /// </summary>
  public bool Contains<T>(MaterialProperty<T> property)
  {
    return _uniforms.ContainsKey(property.Name);
  }

  /// <summary>
  /// Attempts to get a uniform property from the material.
  /// </summary>
  public bool TryGetProperty<T>(MaterialProperty<T> property, out T value)
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
  /// Attempts to get a uniform property from the material, returning a default value if it does not exist.
  /// </summary>
  public T GetPropertyOrDefault<T>(MaterialProperty<T> property, T defaultValue = default!)
  {
    if (!TryGetProperty(property, out var value))
    {
      return defaultValue;
    }

    return value;
  }

  /// <summary>
  /// Attempts to get a uniform property from the material, throwing if it does not exist.
  /// </summary>
  public T GetPropertyOrThrow<T>(MaterialProperty<T> property, T defaultValue = default!)
  {
    if (!TryGetProperty(property, out var value))
    {
      throw new InvalidMaterialPropertyException($"Unable to locate uniform {property.Name}");
    }

    return value;
  }

  /// <summary>
  /// Sets a uniform property on the material.
  /// </summary>
  public void SetProperty<T>(MaterialProperty<T> property, T value)
  {
    _uniforms[property.Name] = Variant.From(value);
  }

  public Dictionary<string, Variant>.Enumerator GetEnumerator()
  {
    return _uniforms.GetEnumerator();
  }

  IEnumerator<KeyValuePair<string, Variant>> IEnumerable<KeyValuePair<string, Variant>>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}

/// <summary>
/// A material is a configuration of the graphics state and properties used for rendering.
/// </summary>
public sealed class Material(IGraphicsBackend backend, ShaderProgram shader, bool ownsShader = true) : GraphicsAsset
{
  /// <summary>
  /// The associated <see cref="ShaderProgram" /> for the material.
  /// </summary>
  public ShaderProgram Shader { get; } = shader;

  /// <summary>
  /// The blend state for this m aterial.
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
  /// The properties of the material.
  /// </summary>
  public MaterialPropertySet Properties { get; } = new();

  /// <summary>
  /// Applies the material properties to the underlying shader.
  /// </summary>
  public void ApplyMaterial()
  {
    // bind the shader
    backend.SetActiveShader(Shader.Handle);

    // apply shader properties
    foreach (var (name, value) in Properties)
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
