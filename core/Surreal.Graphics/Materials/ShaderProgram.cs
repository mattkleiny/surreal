using Surreal.Graphics.Textures;
using Surreal.Maths;

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

  public int GetUniformLocation(string name)
  {
    return backend.GetShaderUniformLocation(Handle, name);
  }

  public bool TryGetUniformLocation(string name, out int location)
  {
    location = GetUniformLocation(name);

    return location != -1;
  }

  public void SetUniform(string name, int value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, float value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Point2 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Point3 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Point4 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Vector2 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Vector3 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Vector4 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Quaternion value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, in Matrix3x2 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      backend.SetShaderUniform(Handle, location, in value);
    }
  }

  public void SetUniform(string name, in Matrix4x4 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      backend.SetShaderUniform(Handle, location, in value);
    }
  }

  public void SetUniform(string name, Texture texture, int slot)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      backend.SetShaderSampler(Handle, location, texture.Handle, slot);
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
