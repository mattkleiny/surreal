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
  public static MaterialProperty<Texture>   DefaultTexture        { get; } = new("u_texture");
  public static MaterialProperty<Matrix4x4> DefaultProjectionView { get; } = new("u_projectionView");

  /// <summary>A global collection of material properties that will be applied to all materials.</summary>
  public static MaterialPropertyCollection Globals { get; } = new();

  private readonly bool ownsShader;
  private MaterialPropertyCollection properties = new();

  public Material(ShaderProgram shader, bool ownsShader = true)
  {
    this.ownsShader = ownsShader;

    Shader = shader;
  }

  /// <summary>The associated <see cref="ShaderProgram"/> for the material.</summary>
  public ShaderProgram Shader { get; }

  /// <summary>The associated properties for the material.</summary>
  public MaterialPropertyCollection Properties
  {
    get => properties;
    set
    {
      // TODO: unset all the old uniforms?
      // TODO: set all of the new uniforms
      properties = value;
    }
  }

  /// <summary>Applies the material properties to the underlying shader.</summary>
  public void Apply()
  {
    // apply globals
    ApplyUniforms(Globals.Uniforms);
    ApplySamplers(Globals.Samplers);

    // apply locals
    ApplyUniforms(properties.Uniforms);
    ApplySamplers(properties.Samplers);
  }

  private void ApplyUniforms(Dictionary<string, Uniform> uniforms)
  {
    foreach (var (name, uniform) in uniforms)
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

        default: throw new InvalidOperationException($"An unexpected uniform value was encountered: {uniform.Kind}");
      }
    }
  }

  private void ApplySamplers(Dictionary<string, Sampler> samplers)
  {
    foreach (var (name, sampler) in samplers)
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
/// <remarks>This collection is not thread-safe.</remarks>
public sealed class MaterialPropertyCollection
{
  internal Dictionary<string, Uniform> Uniforms { get; } = new();
  internal Dictionary<string, Sampler> Samplers { get; } = new();

  // TODO: clean these up?

  public ref int Get(MaterialProperty<int> property)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Kind = UniformKind.Integer;

    return ref uniform.Value.Integer;
  }

  public ref float Get(MaterialProperty<float> property)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Kind = UniformKind.Float;

    return ref uniform.Value.Float;
  }

  public void Set(MaterialProperty<int> property, int value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Kind          = UniformKind.Integer;
    uniform.Value.Integer = value;
  }

  public void Set(MaterialProperty<float> property, float value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Kind        = UniformKind.Float;
    uniform.Value.Float = value;
  }

  public void Set(MaterialProperty<Point2> property, Point2 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Kind         = UniformKind.Point2;
    uniform.Value.Point2 = value;
  }

  public void Set(MaterialProperty<Point3> property, Point3 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Kind         = UniformKind.Point3;
    uniform.Value.Point3 = value;
  }

  public void Set(MaterialProperty<Vector2> property, Vector2 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Kind          = UniformKind.Vector2;
    uniform.Value.Vector2 = value;
  }

  public void Set(MaterialProperty<Vector3> property, Vector3 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Kind          = UniformKind.Vector3;
    uniform.Value.Vector3 = value;
  }

  public void Set(MaterialProperty<Vector4> property, Vector4 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Kind          = UniformKind.Vector4;
    uniform.Value.Vector4 = value;
  }

  public void Set(MaterialProperty<Quaternion> property, Quaternion value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Kind             = UniformKind.Quaternion;
    uniform.Value.Quaternion = value;
  }

  public void Set(MaterialProperty<Matrix3x2> property, in Matrix3x2 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Kind            = UniformKind.Matrix3x2;
    uniform.Value.Matrix3x2 = value;
  }

  public void Set(MaterialProperty<Matrix4x4> property, in Matrix4x4 value)
  {
    ref var uniform = ref Uniforms.GetOrCreateRef(property.Name);

    uniform.Kind            = UniformKind.Matrix4x4;
    uniform.Value.Matrix4x4 = value;
  }

  public void Set(MaterialProperty<Texture> property, Texture texture, int slot = 0)
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
