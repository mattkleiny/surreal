using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Materials;

/// <summary>
/// Standard purpose <see cref="MaterialProperty{T}" />s.
/// </summary>
public static class MaterialProperty
{
  public static MaterialProperty<Texture> Texture { get; } = new("u_texture");
  public static MaterialProperty<Matrix4x4> ProjectionView { get; } = new("u_projectionView");
  public static MaterialProperty<float> Intensity { get; } = new("u_intensity");
}

/// <summary>
/// A strongly typed property name that can be used in a <see cref="Material" />.
/// </summary>
[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly record struct MaterialProperty<T>(string Name);
