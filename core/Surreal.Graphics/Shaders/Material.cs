using System.Runtime.InteropServices;
using Surreal.Collections;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics.Shaders;

/// <summary>A material is a <see cref="ShaderProgram"/> with unique uniform values stored efficiently on the GPU.</summary>
public sealed class Material : GraphicsResource
{
  private readonly Dictionary<string, Uniform> uniforms = new();
  private readonly Dictionary<string, Sampler> samplers = new();
  private readonly ShaderProgram shader;
  private readonly bool ownsShader;

  public Material(ShaderProgram shader, bool ownsShader = true)
  {
    this.shader = shader;
    this.ownsShader = ownsShader;
  }

  public ShaderProgram Shader => shader;

  /// <summary>Uploads the material uniforms to the shader.</summary>
  public void ApplyUniforms()
  {
    foreach (var uniform in uniforms.Values)
    {
      switch (uniform.Kind)
      {
// @formatter:off
        case UniformKind.Integer:    Shader.SetUniform(uniform.Location, uniform.Value.Integer); break;
        case UniformKind.Float:      Shader.SetUniform(uniform.Location, uniform.Value.Float); break;
        case UniformKind.Point2:     Shader.SetUniform(uniform.Location, uniform.Value.Point2); break;
        case UniformKind.Point3:     Shader.SetUniform(uniform.Location, uniform.Value.Point3); break;
        case UniformKind.Vector2:    Shader.SetUniform(uniform.Location, uniform.Value.Vector2); break;
        case UniformKind.Vector3:    Shader.SetUniform(uniform.Location, uniform.Value.Vector3); break;
        case UniformKind.Vector4:    Shader.SetUniform(uniform.Location, uniform.Value.Vector4); break;
        case UniformKind.Quaternion: Shader.SetUniform(uniform.Location, uniform.Value.Quaternion); break;
        case UniformKind.Matrix3x2:  Shader.SetUniform(uniform.Location, in uniform.Value.Matrix3x2); break;
        case UniformKind.Matrix4x4:  Shader.SetUniform(uniform.Location, in uniform.Value.Matrix4x4); break;
// @formatter:on

        default: throw new ArgumentOutOfRangeException();
      }
    }

    foreach (var sampler in samplers.Values)
    {
      Shader.SetUniform(sampler.Location, sampler.Texture, sampler.Slot);
    }
  }

  public void SetUniform(string name, int value)
  {
    ref var uniform = ref uniforms.GetOrCreateRef(name);

    uniform.Location      = shader.GetUniformLocation(name);
    uniform.Kind          = UniformKind.Integer;
    uniform.Value.Integer = value;
  }

  public void SetUniform(string name, float value)
  {
    ref var uniform = ref uniforms.GetOrCreateRef(name);

    uniform.Location    = shader.GetUniformLocation(name);
    uniform.Kind        = UniformKind.Float;
    uniform.Value.Float = value;
  }

  public void SetUniform(string name, Point2 value)
  {
    ref var uniform = ref uniforms.GetOrCreateRef(name);

    uniform.Location     = shader.GetUniformLocation(name);
    uniform.Kind         = UniformKind.Point2;
    uniform.Value.Point2 = value;
  }

  public void SetUniform(string name, Point3 value)
  {
    ref var uniform = ref uniforms.GetOrCreateRef(name);

    uniform.Location     = shader.GetUniformLocation(name);
    uniform.Kind         = UniformKind.Point3;
    uniform.Value.Point3 = value;
  }

  public void SetUniform(string name, Vector2 value)
  {
    ref var uniform = ref uniforms.GetOrCreateRef(name);

    uniform.Location      = shader.GetUniformLocation(name);
    uniform.Kind          = UniformKind.Vector2;
    uniform.Value.Vector2 = value;
  }

  public void SetUniform(string name, Vector3 value)
  {
    ref var uniform = ref uniforms.GetOrCreateRef(name);

    uniform.Location      = shader.GetUniformLocation(name);
    uniform.Kind          = UniformKind.Vector3;
    uniform.Value.Vector3 = value;
  }

  public void SetUniform(string name, Vector4 value)
  {
    ref var uniform = ref uniforms.GetOrCreateRef(name);

    uniform.Location      = shader.GetUniformLocation(name);
    uniform.Kind          = UniformKind.Vector4;
    uniform.Value.Vector4 = value;
  }

  public void SetUniform(string name, Quaternion value)
  {
    ref var uniform = ref uniforms.GetOrCreateRef(name);

    uniform.Location         = shader.GetUniformLocation(name);
    uniform.Kind             = UniformKind.Quaternion;
    uniform.Value.Quaternion = value;
  }

  public void SetUniform(string name, in Matrix3x2 value)
  {
    ref var uniform = ref uniforms.GetOrCreateRef(name);

    uniform.Location        = shader.GetUniformLocation(name);
    uniform.Kind            = UniformKind.Matrix3x2;
    uniform.Value.Matrix3x2 = value;
  }

  public void SetUniform(string name, in Matrix4x4 value)
  {
    ref var uniform = ref uniforms.GetOrCreateRef(name);

    uniform.Location        = shader.GetUniformLocation(name);
    uniform.Kind            = UniformKind.Matrix4x4;
    uniform.Value.Matrix4x4 = value;
  }

  public void SetUniform(string name, Texture texture, int slot = 0)
  {
    ref var sampler = ref samplers.GetOrCreateRef(name);

    sampler.Location = shader.GetUniformLocation(name);
    sampler.Slot     = slot;
    sampler.Texture  = texture;
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      if (ownsShader)
      {
        shader.Dispose();
      }
    }

    base.Dispose(managed);
  }

  /// <summary>A uniform with it's location in the underlying shader.</summary>
  private record struct Uniform
  {
    public int Location;
    public UniformValue Value;
    public UniformKind Kind;
  }

  /// <summary>A sampler with it's location in the underlying shader.</summary>
  private record struct Sampler
  {
    public int Location;
    public int Slot;
    public Texture Texture;

    // TODO: apply these somehow?
    public TextureWrapMode WrapMode;
    public TextureFilterMode MinFilter;
    public TextureFilterMode MagFilter;
  }

  /// <summary>Different kinds of <see cref="UniformValue"/>.</summary>
  private enum UniformKind
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

  /// <summary>A value for a <see cref="Uniform"/>, packed into a union.</summary>
  [StructLayout(LayoutKind.Explicit)]
  private record struct UniformValue
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
