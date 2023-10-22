using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Materials;

/// <summary>
/// A low-level shader program on the GPU.
/// </summary>
public sealed class ShaderProgram(IGraphicsBackend backend) : GraphicsAsset
{
  /// <summary>
  /// The <see cref="GraphicsHandle"/> for the shader itself.
  /// </summary>
  public GraphicsHandle Handle { get; private set; } = backend.CreateShader();

  /// <summary>
  /// Gets the uniform location for the given name.
  /// </summary>
  public int GetUniformLocation(string name)
  {
    return backend.GetShaderUniformLocation(Handle, name);
  }

  /// <summary>
  /// Attempts to get the uniform location for the given name.
  /// </summary>
  public bool TryGetUniformLocation(string name, out int location)
  {
    location = GetUniformLocation(name);

    return location != -1;
  }

  /// <summary>
  /// Sets the uniform value for the given name.
  /// </summary>
  public void SetUniform(string name, Variant value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      // @formatter:off
      switch (value.Type)
      {
        case VariantType.Bool: backend.SetShaderUniform(Handle, location, value.AsBool() ? 1 : 0); break;
        case VariantType.Byte: backend.SetShaderUniform(Handle, location, value.AsByte()); break;
        case VariantType.Short: backend.SetShaderUniform(Handle, location, value.AsShort()); break;
        case VariantType.Ushort: backend.SetShaderUniform(Handle, location, value.AsUshort()); break;
        case VariantType.Int: backend.SetShaderUniform(Handle, location, value.AsInt()); break;
        case VariantType.Uint: backend.SetShaderUniform(Handle, location, value.AsUint()); break;
        case VariantType.Long: backend.SetShaderUniform(Handle, location, value.AsLong()); break;
        case VariantType.Ulong: backend.SetShaderUniform(Handle, location, value.AsUlong()); break;
        case VariantType.Float: backend.SetShaderUniform(Handle, location, value.AsFloat()); break;
        case VariantType.Double: backend.SetShaderUniform(Handle, location, value.AsDouble()); break;
        case VariantType.Vector2: backend.SetShaderUniform(Handle, location, value.AsVector2()); break;
        case VariantType.Vector3: backend.SetShaderUniform(Handle, location, value.AsVector3()); break;
        case VariantType.Vector4: backend.SetShaderUniform(Handle, location, value.AsVector4()); break;
        case VariantType.Quaternion: backend.SetShaderUniform(Handle, location, value.AsQuaternion()); break;
        case VariantType.Color: backend.SetShaderUniform(Handle, location, value.AsColor()); break;
        case VariantType.Color32: backend.SetShaderUniform(Handle, location, value.AsColor32()); break;
        case VariantType.Object when value.AsObject() is Matrix3x2 matrix: backend.SetShaderUniform(Handle, location, matrix); break;
        case VariantType.Object when value.AsObject() is Matrix4x4 matrix: backend.SetShaderUniform(Handle, location, matrix); break;
        case VariantType.Object when value.AsObject() is Texture texture: backend.SetShaderSampler(Handle, location, texture.Handle, 0); break;

        default:
          throw new InvalidMaterialPropertyException($"The material property type for {name} is not supported.");
      }
      // @formatter:on
    }
  }

  /// <summary>
  /// Deletes and replaces the old shader with a new one.
  /// </summary>
  public void ReplaceShader(GraphicsHandle newHandle)
  {
    backend.DeleteShader(Handle);

    Handle = newHandle;
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      backend.DeleteShader(Handle);
    }

    base.Dispose(managed);
  }
}
