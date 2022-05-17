using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using static Surreal.Graphics.Shaders.MaterialPropertyCollection;

namespace Surreal.Graphics.Shaders;

/// <summary>A strongly typed property name that can be used in a <see cref="Material"/>.</summary>
public readonly record struct MaterialProperty<T>(string Name);

/// <summary>A material is a <see cref="ShaderProgram"/> with unique uniform values store on the CPU.</summary>
[DebuggerDisplay("Material (Uniforms {properties.Uniforms.Count}, Samplers {properties.Samplers.Count})")]
public sealed class Material : GraphicsResource
{
  // TODO: offer a global collection of material properties that can be set from anywhere and accessed by all other materials

  private readonly MaterialPropertyCollection properties = new();
  private readonly bool ownsShader;

  public Material(ShaderProgram shader, bool ownsShader = true)
  {
    this.ownsShader = ownsShader;

    Shader = shader;
  }

  public ShaderProgram Shader { get; }

  public void SetProperty<T>(MaterialProperty<T> property, T value)
    => properties.Set(property, value);

  public void SetProperty(string name, int value)
    => properties.Set(name, value);

  public void SetProperty(string name, float value)
    => properties.Set(name, value);

  public void SetProperty(string name, Point2 value)
    => properties.Set(name, value);

  public void SetProperty(string name, Point3 value)
    => properties.Set(name, value);

  public void SetProperty(string name, Vector2 value)
    => properties.Set(name, value);

  public void SetProperty(string name, Vector3 value)
    => properties.Set(name, value);

  public void SetProperty(string name, Vector4 value)
    => properties.Set(name, value);

  public void SetProperty(string name, Quaternion value)
    => properties.Set(name, value);

  public void SetProperty(string name, in Matrix3x2 value)
    => properties.Set(name, in value);

  public void SetProperty(string name, in Matrix4x4 value)
    => properties.Set(name, in value);

  public void SetProperty(string name, Texture texture, int slot = 0)
    => properties.Set(name, texture, slot);

  public void SetProperties(MaterialPropertyCollection collection)
  {
    // TODO: unset all the old uniforms?
    // TODO: set all of the new uniforms
  }

  /// <summary>Applies the material properties to the underlying shader.</summary>
  public void Apply()
  {
    foreach (var (name, uniform) in properties.Uniforms)
    {
      switch (uniform.Kind)
      {
// @formatter:off
        case UniformKind.Integer:    Shader.SetUniform(name, uniform.Value.Integer); break;
        case UniformKind.Float:      Shader.SetUniform(name, uniform.Value.Float); break;
        case UniformKind.Point2:     Shader.SetUniform(name, uniform.Value.Point2); break;
        case UniformKind.Point3:     Shader.SetUniform(name, uniform.Value.Point3); break;
        case UniformKind.Vector2:    Shader.SetUniform(name, uniform.Value.Vector2); break;
        case UniformKind.Vector3:    Shader.SetUniform(name, uniform.Value.Vector3); break;
        case UniformKind.Vector4:    Shader.SetUniform(name, uniform.Value.Vector4); break;
        case UniformKind.Quaternion: Shader.SetUniform(name, uniform.Value.Quaternion); break;
        case UniformKind.Matrix3x2:  Shader.SetUniform(name, in uniform.Value.Matrix3x2); break;
        case UniformKind.Matrix4x4:  Shader.SetUniform(name, in uniform.Value.Matrix4x4); break;
// @formatter:on

        default: throw new ArgumentOutOfRangeException();
      }
    }

    foreach (var (name, sampler) in properties.Samplers)
    {
      Shader.SetUniform(name, sampler.Texture, sampler.TextureSlot);
    }
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

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="Material"/>s.</summary>
public sealed class MaterialLoader : AssetLoader<Material>
{
  public override async Task<Material> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    return new Material(await context.LoadAsync<ShaderProgram>(context.Path, cancellationToken));
  }
}

/// <summary>A collection of properties that can be attached to a <see cref="Material"/>.</summary>
public sealed class MaterialPropertyCollection
{
  internal Dictionary<string, Uniform> Uniforms { get; } = new();
  internal Dictionary<string, Sampler> Samplers { get; } = new();

  public void Set<T>(MaterialProperty<T> property, T value)
  {
    if (typeof(T) == typeof(int)) Set(property.Name, Unsafe.As<T, int>(ref value));
    else if (typeof(T) == typeof(float)) Set(property.Name, Unsafe.As<T, float>(ref value));
    else if (typeof(T) == typeof(Point2)) Set(property.Name, Unsafe.As<T, Point2>(ref value));
    else if (typeof(T) == typeof(Point3)) Set(property.Name, Unsafe.As<T, Point3>(ref value));
    else if (typeof(T) == typeof(Vector2)) Set(property.Name, Unsafe.As<T, Vector2>(ref value));
    else if (typeof(T) == typeof(Vector3)) Set(property.Name, Unsafe.As<T, Vector3>(ref value));
    else if (typeof(T) == typeof(Vector4)) Set(property.Name, Unsafe.As<T, Vector4>(ref value));
    else if (typeof(T) == typeof(Quaternion)) Set(property.Name, Unsafe.As<T, Quaternion>(ref value));
    else if (typeof(T) == typeof(Matrix3x2)) Set(property.Name, in Unsafe.As<T, Matrix3x2>(ref value));
    else if (typeof(T) == typeof(Matrix4x4)) Set(property.Name, in Unsafe.As<T, Matrix4x4>(ref value));
    else if (typeof(T) == typeof(Texture)) Set(property.Name, Unsafe.As<T, Texture>(ref value));
    else
    {
      throw new InvalidOperationException($"An unrecognized material property type was requested: {property}");
    }
  }

  public void Set(string name, int value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(name);

    uniform.Kind          = UniformKind.Integer;
    uniform.Value.Integer = value;
  }

  public void Set(string name, float value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(name);

    uniform.Kind        = UniformKind.Float;
    uniform.Value.Float = value;
  }

  public void Set(string name, Point2 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(name);

    uniform.Kind         = UniformKind.Point2;
    uniform.Value.Point2 = value;
  }

  public void Set(string name, Point3 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(name);

    uniform.Kind         = UniformKind.Point3;
    uniform.Value.Point3 = value;
  }

  public void Set(string name, Vector2 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(name);

    uniform.Kind          = UniformKind.Vector2;
    uniform.Value.Vector2 = value;
  }

  public void Set(string name, Vector3 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(name);

    uniform.Kind          = UniformKind.Vector3;
    uniform.Value.Vector3 = value;
  }

  public void Set(string name, Vector4 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(name);

    uniform.Kind          = UniformKind.Vector4;
    uniform.Value.Vector4 = value;
  }

  public void Set(string name, Quaternion value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(name);

    uniform.Kind             = UniformKind.Quaternion;
    uniform.Value.Quaternion = value;
  }

  public void Set(string name, in Matrix3x2 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(name);

    uniform.Kind            = UniformKind.Matrix3x2;
    uniform.Value.Matrix3x2 = value;
  }

  public void Set(string name, in Matrix4x4 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(name);

    uniform.Kind            = UniformKind.Matrix4x4;
    uniform.Value.Matrix4x4 = value;
  }

  public void Set(string name, Texture texture, int slot = 0)
  {
    ref var sampler = ref Samplers.GetOrCreateRef(name);

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
    public UniformKind Kind;
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

  /// <summary>Different kinds of <see cref="UniformValue"/>.</summary>
  internal enum UniformKind
  {
    Integer,
    Float,
    Point2,
    Point3,
    Vector2,
    Vector3,
    Vector4,
    Quaternion,
    Matrix3x2,
    Matrix4x4,
  }

  /// <summary>A single value for a <see cref="Uniform"/>, packed into a union.</summary>
  [StructLayout(LayoutKind.Explicit)]
  internal record struct UniformValue
  {
    [FieldOffset(0)] public int Integer;
    [FieldOffset(0)] public float Float;
    [FieldOffset(0)] public Point2 Point2;
    [FieldOffset(0)] public Point3 Point3;
    [FieldOffset(0)] public Vector2 Vector2;
    [FieldOffset(0)] public Vector3 Vector3;
    [FieldOffset(0)] public Vector4 Vector4;
    [FieldOffset(0)] public Quaternion Quaternion;
    [FieldOffset(0)] public Matrix3x2 Matrix3x2;
    [FieldOffset(0)] public Matrix4x4 Matrix4x4;
  }
}
