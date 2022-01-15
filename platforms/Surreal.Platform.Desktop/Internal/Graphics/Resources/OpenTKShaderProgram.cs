using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Mathematics;

namespace Surreal.Internal.Graphics.Resources;

internal sealed class OpenTKShaderProgram : ShaderProgram
{
  private readonly Dictionary<string, int> locationCache = new();

  public ProgramHandle Id { get; } = GL.CreateProgram();

  public OpenTKShaderProgram(OpenTKShaderSet shaders)
  {
    Link(shaders);
  }

  public void Bind()
  {
    GL.UseProgram(Id);
  }

  public override void Bind(VertexDescriptorSet descriptors)
  {
    GL.UseProgram(Id);

    for (var i = 0; i < descriptors.Length; i++)
    {
      var attribute = descriptors[i];
      var location  = GL.GetAttribLocation(Id, attribute.Alias);

      if (location == -1) continue; // attribute undefined in the shader? just move on

      GL.VertexAttribPointer(
        index: (uint) location,
        size: attribute.Count,
        type: ConvertVertexType(attribute.Type),
        normalized: attribute.Normalized,
        stride: descriptors.Stride,
        offset: attribute.Offset
      );
      GL.EnableVertexAttribArray((uint) location);
    }
  }

  private void Link(OpenTKShaderSet shaderSet)
  {
    var shaders   = shaderSet.Shaders;
    var shaderIds = new ShaderHandle[shaders.Length];

    GL.UseProgram(Id);

    for (var i = 0; i < shaders.Length; i++)
    {
      var shader = shaders[i];
      var code   = shader.Code;

      var handle = shaderIds[i] = GL.CreateShader(shader.Type);

      GL.ShaderSource(handle, code);
      GL.CompileShader(handle);

      var compileStatus = 0;

      GL.GetShaderi(handle, ShaderParameterName.CompileStatus, ref compileStatus);

      if (compileStatus != 1)
      {
        GL.GetShaderInfoLog(handle, out var errorLog);

        throw new ShaderProgramException($"An error occurred whilst compiling a {shader.Type} shader.", errorLog);
      }

      GL.AttachShader(Id, handle);
    }

    int linkStatus = 0;

    GL.LinkProgram(Id);
    GL.GetProgrami(Id, ProgramPropertyARB.LinkStatus, ref linkStatus);

    if (linkStatus != 1)
    {
      GL.GetProgramInfoLog(Id, out var errorLog);

      throw new ShaderProgramException("An error occurred whilst linking a shader program.", errorLog);
    }

    foreach (var shaderId in shaderIds)
    {
      GL.DeleteShader(shaderId);
    }
  }

  public override void SetUniform(string name, int scalar)            => GL.Uniform1i(GetUniformLocation(name), scalar);
  public override void SetUniform(string name, float scalar)          => GL.Uniform1f(GetUniformLocation(name), scalar);
  public override void SetUniform(string name, Vector2I point)        => GL.Uniform2i(GetUniformLocation(name), point.X, point.Y);
  public override void SetUniform(string name, Vector3I point)        => GL.Uniform3i(GetUniformLocation(name), point.X, point.Y, point.Z);
  public override void SetUniform(string name, Vector2 vector)        => GL.Uniform2f(GetUniformLocation(name), vector.X, vector.Y);
  public override void SetUniform(string name, Vector3 vector)        => GL.Uniform3f(GetUniformLocation(name), vector.X, vector.Y, vector.Z);
  public override void SetUniform(string name, Vector4 vector)        => GL.Uniform4f(GetUniformLocation(name), vector.X, vector.Y, vector.Z, vector.W);
  public override void SetUniform(string name, Quaternion quaternion) => GL.Uniform4f(GetUniformLocation(name), quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);

  public override unsafe void SetUniform(string name, in Matrix3x2 matrix)
  {
    var location = GetUniformLocation(name);

    ref var source   = ref Unsafe.AsRef(in matrix);
    var     elements = (float*) Unsafe.AsPointer(ref source);
    var     span     = new ReadOnlySpan<float>(elements, 3 * 2);

    GL.UniformMatrix4f(location, 1, false, span);
  }

  public override unsafe void SetUniform(string name, in Matrix4x4 matrix)
  {
    var location = GetUniformLocation(name);

    ref var source   = ref Unsafe.AsRef(in matrix);
    var     elements = (float*) Unsafe.AsPointer(ref source);
    var     span     = new ReadOnlySpan<float>(elements, 4 * 4);

    GL.UniformMatrix4f(location, 1, false, span);
  }

  protected override void Dispose(bool managed)
  {
    GL.DeleteProgram(Id);

    base.Dispose(managed);
  }

  private int GetUniformLocation(string name)
  {
    if (!locationCache.TryGetValue(name, out var id))
    {
      id = locationCache[name] = GL.GetUniformLocation(Id, name);
    }

    return id;
  }

  private static VertexAttribPointerType ConvertVertexType(VertexType type)
  {
    return type switch
    {
      VertexType.UnsignedByte  => VertexAttribPointerType.UnsignedByte,
      VertexType.Byte          => VertexAttribPointerType.Byte,
      VertexType.Short         => VertexAttribPointerType.Short,
      VertexType.UnsignedShort => VertexAttribPointerType.UnsignedShort,
      VertexType.Int           => VertexAttribPointerType.Int,
      VertexType.UnsignedInt   => VertexAttribPointerType.UnsignedInt,
      VertexType.Float         => VertexAttribPointerType.Float,
      VertexType.Double        => VertexAttribPointerType.Double,

      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
  }
}

internal sealed class ShaderProgramException : PlatformException
{
  public ShaderProgramException(string message, string informationLog)
    : base(message)
  {
    InformationLog = informationLog;
  }

  public string InformationLog { get; }
}
