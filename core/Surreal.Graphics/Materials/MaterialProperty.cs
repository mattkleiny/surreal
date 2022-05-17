using System.Runtime.InteropServices;
using Surreal.Collections;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics.Materials;

/// <summary>Standard purpose <see cref="MaterialProperty{T}"/>s.</summary>
public static class MaterialProperty
{
  public static MaterialProperty<Texture>   Texture        { get; } = new("u_texture");
  public static MaterialProperty<Matrix4x4> ProjectionView { get; } = new("u_projectionView");
  public static MaterialProperty<float>     Intensity      { get; } = new("u_intensity");
}

/// <summary>A strongly typed property name that can be used in a <see cref="Material"/>.</summary>
[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly record struct MaterialProperty<T>(string Name);

/// <summary>A collection of properties that can be attached to a <see cref="Material"/>.</summary>
/// <remarks>This collection is not thread-safe.</remarks>
public sealed class MaterialPropertySet
{
  internal Dictionary<string, Uniform> Uniforms { get; } = new();
  internal Dictionary<string, Sampler> Samplers { get; } = new();

  // TODO: clean this API up?
  public ref float GetProperty(MaterialProperty<float> property)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type = UniformType.Float;

    return ref uniform.Value.Float;
  }

  public void SetProperty(MaterialProperty<int> property, int value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type          = UniformType.Integer;
    uniform.Value.Integer = value;
  }

  public void SetProperty(MaterialProperty<float> property, float value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type        = UniformType.Float;
    uniform.Value.Float = value;
  }

  public void SetProperty(MaterialProperty<Point2> property, Point2 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type         = UniformType.Point2;
    uniform.Value.Point2 = value;
  }

  public void SetProperty(MaterialProperty<Point3> property, Point3 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type         = UniformType.Point3;
    uniform.Value.Point3 = value;
  }

  public void SetProperty(MaterialProperty<Point4> property, Point4 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type         = UniformType.Point4;
    uniform.Value.Point4 = value;
  }

  public void SetProperty(MaterialProperty<Color32> property, Color32 color)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type         = UniformType.Point3;
    uniform.Value.Point4 = (Point4) color;
  }

  public void SetProperty(MaterialProperty<Vector2> property, Vector2 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type          = UniformType.Vector2;
    uniform.Value.Vector2 = value;
  }

  public void SetProperty(MaterialProperty<Vector3> property, Vector3 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type          = UniformType.Vector3;
    uniform.Value.Vector3 = value;
  }

  public void SetProperty(MaterialProperty<Vector4> property, Vector4 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type          = UniformType.Vector4;
    uniform.Value.Vector4 = value;
  }

  public void SetProperty(MaterialProperty<Color> property, Color color)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type          = UniformType.Vector4;
    uniform.Value.Vector4 = (Vector4) color;
  }

  public void SetProperty(MaterialProperty<Quaternion> property, Quaternion value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type             = UniformType.Quaternion;
    uniform.Value.Quaternion = value;
  }

  public void SetProperty(MaterialProperty<Matrix3x2> property, in Matrix3x2 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type            = UniformType.Matrix3x2;
    uniform.Value.Matrix3x2 = value;
  }

  public void SetProperty(MaterialProperty<Matrix4x4> property, in Matrix4x4 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Type            = UniformType.Matrix4x4;
    uniform.Value.Matrix4x4 = value;
  }

  public void SetProperty(MaterialProperty<Texture> property, Texture texture, int slot = 0)
  {
    ref var sampler = ref Samplers.GetOrCreateRef(property.Name);

    sampler.Texture     = texture;
    sampler.TextureSlot = slot;
  }

  /// <summary>Clear all properties from the collection.</summary>
  public void Clear()
  {
    Uniforms.Clear();
    Samplers.Clear();
  }

  /// <summary>A uniform value that can be bound to an underlying shader program.</summary>
  internal record struct Uniform
  {
    public UniformValue Value;
    public UniformType Type;
  }

  /// <summary>A sampler value that can be bound to an underlying shader program.</summary>
  internal record struct Sampler
  {
    public Texture Texture;
    public int TextureSlot;

    // TODO: apply these somehow?
    public TextureWrapMode WrapMode;
    public TextureFilterMode MinFilter;
    public TextureFilterMode MagFilter;
  }

  /// <summary>A single value for a <see cref="Uniform"/>, packed into a union.</summary>
  [StructLayout(LayoutKind.Explicit)]
  internal record struct UniformValue
  {
    [FieldOffset(0)] public int Integer;
    [FieldOffset(0)] public float Float;
    [FieldOffset(0)] public Point2 Point2;
    [FieldOffset(0)] public Point3 Point3;
    [FieldOffset(0)] public Point4 Point4;
    [FieldOffset(0)] public Vector2 Vector2;
    [FieldOffset(0)] public Vector3 Vector3;
    [FieldOffset(0)] public Vector4 Vector4;
    [FieldOffset(0)] public Quaternion Quaternion;
    [FieldOffset(0)] public Matrix3x2 Matrix3x2;
    [FieldOffset(0)] public Matrix4x4 Matrix4x4;
  }
}
