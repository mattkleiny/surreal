using Surreal.Colors;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Materials;

/// <summary>
/// Indicates that a <see cref="MaterialProperty{T}"/> does not exist on a material.
/// </summary>
public sealed class InvalidMaterialPropertyException(string? message = "An invalid material property was specified") : ApplicationException(message);

/// <summary>
/// Commonly used <see cref="MaterialProperty{T}"/>s.
/// </summary>
public static class MaterialProperty
{
  public static MaterialProperty<Matrix4x4> Transform { get; } = new("u_transform");
  public static MaterialProperty<TextureSampler> Texture { get; } = new("u_texture");
  public static MaterialProperty<Color> Color { get; } = new("u_color");
}

/// <summary>
/// A property of a <see cref="Material" />.
/// </summary>
[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly record struct MaterialProperty<T>(string Name)
{
  public static implicit operator MaterialProperty<T>(string value) => new(value);
}

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
  public T GetPropertyOrThrow<T>(MaterialProperty<T> property)
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
