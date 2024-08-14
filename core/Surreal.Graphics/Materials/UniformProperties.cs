using Surreal.Colors;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Materials;

/// <summary>
/// Indicates that a <see cref="UniformProperty{T}"/> does not exist on a material.
/// </summary>
public sealed class InvalidUniformException(string? message = "An invalid uniform was specified") : ApplicationException(message);

/// <summary>
/// Commonly used <see cref="UniformProperty{T}"/>s.
/// </summary>
public static class UniformProperty
{
  public static UniformProperty<Matrix4x4> Transform { get; } = new("u_transform");
  public static UniformProperty<TextureSampler> Texture { get; } = new("u_texture");
  public static UniformProperty<Color> Color { get; } = new("u_color");
}

/// <summary>
/// A property of a <see cref="Material" />.
/// </summary>
[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly record struct UniformProperty<T>(string Name)
{
  public static implicit operator UniformProperty<T>(string value) => new(value);
}

/// <summary>
/// A set of <see cref="UniformProperty{T}" />s in a <see cref="Material" />.
/// </summary>
[DebuggerDisplay("UniformSet ({_uniforms.Count} uniforms)")]
public sealed class UniformSet : IEnumerable<KeyValuePair<string, Variant>>
{
  private readonly Dictionary<string, Variant> _uniforms = new();

  /// <summary>
  /// Determines if the material contains the given property.
  /// </summary>
  public bool Contains<T>(UniformProperty<T> property)
  {
    return _uniforms.ContainsKey(property.Name);
  }

  /// <summary>
  /// Attempts to get a property from the material.
  /// </summary>
  public bool TryGet<T>(UniformProperty<T> property, out T value)
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
  /// Attempts to get a property from the material, returning a default value if it does not exist.
  /// </summary>
  public T GetOrDefault<T>(UniformProperty<T> property, T defaultValue = default!)
  {
    if (!TryGet(property, out var value))
    {
      return defaultValue;
    }

    return value;
  }

  /// <summary>
  /// Attempts to get a property from the material, throwing if it does not exist.
  /// </summary>
  public T GetOrThrow<T>(UniformProperty<T> property)
  {
    if (!TryGet(property, out var value))
    {
      throw new InvalidUniformException($"Unable to locate uniform {property.Name}");
    }

    return value;
  }

  /// <summary>
  /// Adds a property to the material.
  /// </summary>
  public void Add<T>(UniformProperty<T> property, T value)
  {
    _uniforms.Add(property.Name, Variant.From(value));
  }

  /// <summary>
  /// Sets a property on the material.
  /// </summary>
  public void Set<T>(UniformProperty<T> property, T value)
  {
    _uniforms[property.Name] = Variant.From(value);
  }

  /// <summary>
  /// Removes a property from the material.
  /// </summary>
  public void Remove<T>(UniformProperty<T> property)
  {
    _uniforms.Remove(property.Name);
  }

  /// <summary>
  /// Clears all uniform properties from the material.
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
