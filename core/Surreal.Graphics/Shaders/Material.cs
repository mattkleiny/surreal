using System.Runtime.InteropServices;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using static Surreal.Graphics.Shaders.MaterialPropertySet;

namespace Surreal.Graphics.Shaders;

/// <summary>Different kinds of blends.</summary>
public enum BlendMode
{
  SourceColor,
  TargetColor,
  SourceAlpha,
  TargetAlpha,
  OneMinusSourceColor,
  OneMinusTargetColor,
  OneMinusSourceAlpha,
  OneMinusTargetAlpha,
}

/// <summary>State efor a blending operation.</summary>
public readonly record struct BlendState(bool IsEnabled, BlendMode Source, BlendMode Target)
{
  public static BlendState Disabled            { get; } = default(BlendState) with { IsEnabled = false };
  public static BlendState OneMinusSourceAlpha { get; } = new(true, BlendMode.SourceAlpha, BlendMode.OneMinusSourceAlpha);
}

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

/// <summary>A material is a <see cref="ShaderProgram"/> with unique uniform values store on the CPU.</summary>
[DebuggerDisplay("Material (Uniforms {Locals.Uniforms.Count}, Samplers {properties.Samplers.Count})")]
public sealed class Material : GraphicsResource
{
  /// <summary>Global properties shared amongst all <see cref="Material"/>s.</summary>
  public static MaterialPropertySet Globals { get; } = new();

  private readonly bool ownsShader;

  public Material(ShaderProgram shader, bool ownsShader = true)
  {
    this.ownsShader = ownsShader;

    Shader = shader;
  }

  /// <summary>The associated <see cref="ShaderProgram"/> for the material.</summary>
  public ShaderProgram Shader { get; }

  /// <summary>The properties associated with this material.</summary>
  public MaterialPropertySet Locals { get; } = new();

  /// <summary>The desired <see cref="BlendState"/> for this material.</summary>
  public BlendState Blending { get; set; } = BlendState.OneMinusSourceAlpha;

  /// <summary>Applies the material properties to the underlying shader.</summary>
  public void Apply(IGraphicsServer server)
  {
    // bind the shader
    server.SetActiveShader(Shader.Handle);

    // apply globals
    ApplyUniforms(Globals.Uniforms);
    ApplySamplers(Globals.Samplers);

    // apply locals
    ApplyUniforms(Locals.Uniforms);
    ApplySamplers(Locals.Samplers);

    // apply blend state
    server.SetBlendState(Blending);
  }

  private void ApplyUniforms(Dictionary<string, Uniform> uniforms)
  {
    foreach (var (name, uniform) in uniforms)
    {
      switch (uniform.Type)
      {
// @formatter:off
        case UniformType.Integer:    Shader.SetUniform(name, uniform.Value.Integer); break;
        case UniformType.Float:      Shader.SetUniform(name, uniform.Value.Float); break;
        case UniformType.Point2:     Shader.SetUniform(name, uniform.Value.Point2); break;
        case UniformType.Point3:     Shader.SetUniform(name, uniform.Value.Point3); break;
        case UniformType.Point4:     Shader.SetUniform(name, uniform.Value.Point4); break;
        case UniformType.Vector2:    Shader.SetUniform(name, uniform.Value.Vector2); break;
        case UniformType.Vector3:    Shader.SetUniform(name, uniform.Value.Vector3); break;
        case UniformType.Vector4:    Shader.SetUniform(name, uniform.Value.Vector4); break;
        case UniformType.Quaternion: Shader.SetUniform(name, uniform.Value.Quaternion); break;
        case UniformType.Matrix3x2:  Shader.SetUniform(name, in uniform.Value.Matrix3x2); break;
        case UniformType.Matrix4x4:  Shader.SetUniform(name, in uniform.Value.Matrix4x4); break;
// @formatter:on

        default: throw new InvalidOperationException($"An unexpected uniform value was encountered: {uniform.Type}");
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
public sealed class MaterialPropertySet
{
  internal Dictionary<string, Uniform> Uniforms { get; } = new();
  internal Dictionary<string, Sampler> Samplers { get; } = new();

  public ref float Access(MaterialProperty<float> property)
  {
    // TODO: clean this up?
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
