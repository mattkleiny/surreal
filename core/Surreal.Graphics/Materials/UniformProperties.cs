using Surreal.Colors;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Materials;

/// <summary>
/// Indicates that a <see cref="ShaderProperty{T}"/> does not exist on a material.
/// </summary>
public sealed class InvalidShaderPropertyException(string? message = "An invalid shader property was specified") : ApplicationException(message);

/// <summary>
/// Commonly used <see cref="ShaderProperty{T}"/>s.
/// </summary>
public static class ShaderProperty
{
  public static ShaderProperty<Matrix4x4> Transform { get; } = new("u_transform");
  public static ShaderProperty<TextureSampler> Texture { get; } = new("u_texture");
  public static ShaderProperty<Color> Color { get; } = new("u_color");
}

/// <summary>
/// A property of a <see cref="Material"/>.
/// </summary>
[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly record struct ShaderProperty<T>(string Name)
{
  public static implicit operator ShaderProperty<T>(string value) => new(value);
}

/// <summary>
/// A set of <see cref="ShaderProperty{T}"/>s in a <see cref="Material" />.
/// </summary>
[DebuggerDisplay("UniformSet ({_uniforms.Count} uniforms)")]
public sealed class ShaderPropertySet : IEnumerable<KeyValuePair<string, Variant>>
{
  private readonly Dictionary<string, Variant> _uniforms = new();

  /// <summary>
  /// Determines if the set contains the given property.
  /// </summary>
  public bool Contains<T>(ShaderProperty<T> property)
  {
    return _uniforms.ContainsKey(property.Name);
  }

  /// <summary>
  /// Attempts to get a property from the set.
  /// </summary>
  public bool TryGet<T>(ShaderProperty<T> property, out T value)
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
  /// Attempts to get a property from the set, returning a default value if it does not exist.
  /// </summary>
  public T GetOrDefault<T>(ShaderProperty<T> property, T defaultValue = default!)
  {
    if (!TryGet(property, out var value))
    {
      return defaultValue;
    }

    return value;
  }

  /// <summary>
  /// Attempts to get a property from the set, throwing if it does not exist.
  /// </summary>
  public T GetOrThrow<T>(ShaderProperty<T> property)
  {
    if (!TryGet(property, out var value))
    {
      throw new InvalidShaderPropertyException($"Unable to locate uniform {property.Name}");
    }

    return value;
  }

  /// <summary>
  /// Adds a property to the set.
  /// </summary>
  public void Add<T>(ShaderProperty<T> property, T value)
  {
    _uniforms.Add(property.Name, Variant.From(value));
  }

  /// <summary>
  /// Sets a property on the set.
  /// </summary>
  public void Set<T>(ShaderProperty<T> property, T value)
  {
    _uniforms[property.Name] = Variant.From(value);
  }

  /// <summary>
  /// Removes a property from the set.
  /// </summary>
  public void Remove<T>(ShaderProperty<T> property)
  {
    _uniforms.Remove(property.Name);
  }

  /// <summary>
  /// Clears all uniform properties from the set.
  /// </summary>
  public void Clear()
  {
    _uniforms.Clear();
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
