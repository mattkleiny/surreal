using Surreal.Collections;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics.Shaders;

/// <summary>
/// Different kinds of uniform values supported on a <see cref="ShaderProgram" />.
/// </summary>
public enum UniformType
{
  Integer,
  Float,
  Point2,
  Point3,
  Point4,
  Vector2,
  Vector3,
  Vector4,
  Quaternion,
  Matrix3X2,
  Matrix4X4,
  Texture
}

/// <summary>
/// Metadata about attributes in a <see cref="ShaderProgram" />.
/// </summary>
public sealed record ShaderAttributeMetadata(
  string Name,
  int Location,
  int Length,
  int Count,
  UniformType Type
);

/// <summary>
/// Metadata about uniforms in a <see cref="ShaderProgram" />.
/// </summary>
public sealed record ShaderUniformMetadata(
  string Name,
  int Location,
  int Length,
  int Count,
  UniformType Type
);

/// <summary>
/// A low-level shader program on the GPU.
/// </summary>
public sealed class ShaderProgram : GraphicsResource
{
  public ShaderProgram(IGraphicsServer server)
  {
    Server = server;
    Handle = server.Backend.CreateShader();
  }

  public IGraphicsServer Server { get; }
  public GraphicsHandle Handle { get; private set; }

  /// <summary>
  /// The attributes in the shader.
  /// </summary>
  public ReadOnlySlice<ShaderAttributeMetadata> Attributes { get; private set; } = ReadOnlySlice<ShaderAttributeMetadata>.Empty;

  /// <summary>
  /// The uniforms in the shader.
  /// </summary>
  public ReadOnlySlice<ShaderUniformMetadata> Uniforms { get; private set; } = ReadOnlySlice<ShaderUniformMetadata>.Empty;

  public int GetUniformLocation(string name)
  {
    return Server.Backend.GetShaderUniformLocation(Handle, name);
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
      Server.Backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, float value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      Server.Backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Point2 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      Server.Backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Point3 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      Server.Backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Point4 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      Server.Backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Vector2 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      Server.Backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Vector3 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      Server.Backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Vector4 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      Server.Backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, Quaternion value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      Server.Backend.SetShaderUniform(Handle, location, value);
    }
  }

  public void SetUniform(string name, in Matrix3x2 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      Server.Backend.SetShaderUniform(Handle, location, in value);
    }
  }

  public void SetUniform(string name, in Matrix4x4 value)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      Server.Backend.SetShaderUniform(Handle, location, in value);
    }
  }

  public void SetUniform(string name, Texture texture, int slot)
  {
    if (TryGetUniformLocation(name, out var location))
    {
      Server.Backend.SetShaderSampler(Handle, location, texture.Handle, slot);
    }
  }

  /// <summary>
  /// Deletes and replaces the old shader with a new one.
  /// </summary>
  public void ReplaceShader(GraphicsHandle newHandle)
  {
    Server.Backend.DeleteShader(Handle);

    Handle = newHandle;
  }

  /// <summary>
  /// Updates the attribute/uniform metadata for the shader.
  /// </summary>
  public void ReloadMetadata()
  {
    Attributes = Server.Backend.GetShaderAttributeMetadata(Handle);
    Uniforms = Server.Backend.GetShaderUniformMetadata(Handle);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      Server.Backend.DeleteShader(Handle);
    }

    base.Dispose(managed);
  }
}
