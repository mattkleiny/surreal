using Surreal.Graphics.Shaders;
using static Surreal.Graphics.Materials.MaterialPropertyCollection;

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
/// State blending operation.
/// </summary>
public readonly record struct BlendState(bool IsEnabled, BlendMode Source, BlendMode Target)
{
  public static BlendState Disabled { get; } = default(BlendState) with { IsEnabled = false };

  public static BlendState OneMinusSourceAlpha { get; } =
    new(true, BlendMode.SourceAlpha, BlendMode.OneMinusSourceAlpha);
}

/// <summary>
/// A material is a configuration of the graphics state and properties used for rendering.
/// </summary>
[DebuggerDisplay("Material (Uniforms {Properties.Uniforms.Count}, Samplers {Properties.Samplers.Count})")]
public sealed class Material(ShaderProgram shader, bool ownsShader = true) : GraphicsResource
{
  /// <summary>
  /// The underlying <see cref="GraphicsContext" />.
  /// </summary>
  public GraphicsContext Context => Shader.Context;

  /// <summary>
  /// The associated <see cref="ShaderProgram" /> for the material.
  /// </summary>
  public ShaderProgram Shader { get; } = shader;

  /// <summary>
  /// The properties associated with this material.
  /// </summary>
  public MaterialPropertyCollection Properties { get; } = new();

  /// <summary>
  /// The desired <see cref="BlendState" /> for this material.
  /// </summary>
  public BlendState Blending { get; set; } = BlendState.OneMinusSourceAlpha;

  /// <summary>
  /// Applies the material properties to the underlying shader.
  /// </summary>
  public void Apply(GraphicsContext context)
  {
    // bind the shader
    context.Backend.SetActiveShader(Shader.Handle);

    // apply locals
    ApplyUniforms(Properties.Uniforms);
    ApplySamplers(Properties.Samplers);

    // apply blend state
    context.Backend.SetBlendState(Blending);
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
        case UniformType.Matrix3X2:  Shader.SetUniform(name, in uniform.Value.Matrix3x2); break;
        case UniformType.Matrix4X4:  Shader.SetUniform(name, in uniform.Value.Matrix4x4); break;

        // @formatter:on
        default: throw new InvalidOperationException($"An unexpected uniform value was encountered: {uniform.Type}");
      }
    }
  }

  private void ApplySamplers(Dictionary<string, Sampler> samplers)
  {
    foreach (var (name, sampler) in samplers)
    {
      if (sampler.Texture != null)
      {
        Shader.SetUniform(name, sampler.Texture, sampler.TextureSlot);
      }
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
