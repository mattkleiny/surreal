using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics.Linear;
using ShaderType = OpenTK.Graphics.OpenGL.ShaderType;

namespace Surreal.Platform.Internal.Graphics.Resources {
  internal sealed class OpenTKShaderProgram : ShaderProgram {
    private readonly Dictionary<string, int> locationCache = new Dictionary<string, int>();

    public readonly int Id = GL.CreateProgram();

    public OpenTKShaderProgram(params Shader[] shaders) {
      Link(shaders);
    }

    public void Bind() {
      GL.UseProgram(Id);
    }

    public override void Bind(VertexAttributes attributes) {
      GL.UseProgram(Id);

      for (var i = 0; i < attributes.Length; i++) {
        var attribute = attributes[i];
        var location  = GL.GetAttribLocation(Id, attribute.Alias);

        if (location == -1) continue; // attribute undefined in the shader? just move on

        GL.VertexAttribPointer(
            index: location,
            size: attribute.Count,
            type: ConvertVertexType(attribute.Type),
            normalized: attribute.Normalized,
            stride: attributes.Stride,
            offset: attribute.Offset
        );
        GL.EnableVertexAttribArray(location);
      }
    }

    private void Link(ReadOnlySpan<Shader> shaders) {
      var shaderIds = new int[shaders.Length];

      GL.UseProgram(Id);

      for (var i = 0; i < shaders.Length; i++) {
        var shader = shaders[i];
        var code   = Encoding.UTF8.GetString(shader.Raw);

        var shaderId = shaderIds[i] = GL.CreateShader(ConvertShaderType(shader.Type));

        GL.ShaderSource(shaderId, code);
        GL.CompileShader(shaderId);

        GL.GetShader(shaderId, ShaderParameter.CompileStatus, out var status);

        if (status != 1) {
          GL.GetShaderInfoLog(shaderId, out var errorLog);

          throw new ShaderProgramException($"An error occurred whilst compiling a {shader.Type} shader.", errorLog);
        }

        GL.AttachShader(Id, shaderId);
      }

      GL.LinkProgram(Id);
      GL.GetProgram(Id, GetProgramParameterName.LinkStatus, out var linkStatus);

      if (linkStatus != 1) {
        GL.GetProgramInfoLog(Id, out var errorLog);

        throw new ShaderProgramException("An error occurred whilst linking a shader program.", errorLog);
      }

      foreach (var shaderId in shaderIds) {
        GL.DeleteShader(shaderId);
      }
    }

    private int GetUniformLocation(string alias) {
      if (!locationCache.TryGetValue(alias, out var id)) {
        id = locationCache[alias] = GL.GetUniformLocation(Id, alias);
      }

      return id;
    }

    public override void SetUniform(string alias, int scalar) {
      var location = GetUniformLocation(alias);

      GL.Uniform1(location, scalar);
    }

    public override void SetUniform(string alias, float scalar) {
      var location = GetUniformLocation(alias);

      GL.Uniform1(location, scalar);
    }

    public override void SetUniform(string alias, Vector2I point) {
      var location = GetUniformLocation(alias);

      GL.Uniform2(location, point.X, point.Y);
    }

    public override void SetUniform(string alias, Vector3I point) {
      var location = GetUniformLocation(alias);

      GL.Uniform3(location, point.X, point.Y, point.Z);
    }

    public override void SetUniform(string alias, Vector2 vector) {
      var location = GetUniformLocation(alias);

      GL.Uniform2(location, vector.X, vector.Y);
    }

    public override void SetUniform(string alias, Vector3 vector) {
      var location = GetUniformLocation(alias);

      GL.Uniform3(location, vector.X, vector.Y, vector.Z);
    }

    public override void SetUniform(string alias, Vector4 vector) {
      var location = GetUniformLocation(alias);

      GL.Uniform4(location, vector.W, vector.X, vector.Y, vector.Z);
    }

    public override void SetUniform(string alias, Quaternion quaternion) {
      var location = GetUniformLocation(alias);

      GL.Uniform4(location, quaternion.W, quaternion.X, quaternion.Y, quaternion.Z);
    }

    public override unsafe void SetUniform(string alias, in Matrix2x2 matrix) {
      var location = GetUniformLocation(alias);

      ref var source   = ref Unsafe.AsRef(in matrix);
      var     elements = (float*) Unsafe.AsPointer(ref source);

      GL.UniformMatrix4(location, 1, false, elements);
    }

    public override unsafe void SetUniform(string alias, in Matrix3x2 matrix) {
      var location = GetUniformLocation(alias);

      ref var source   = ref Unsafe.AsRef(in matrix);
      var     elements = (float*) Unsafe.AsPointer(ref source);

      GL.UniformMatrix4(location, 1, false, elements);
    }

    public override unsafe void SetUniform(string alias, in Matrix4x4 matrix) {
      var location = GetUniformLocation(alias);

      ref var source   = ref Unsafe.AsRef(in matrix);
      var     elements = (float*) Unsafe.AsPointer(ref source);

      GL.UniformMatrix4(location, 1, false, elements);
    }

    protected override void Dispose(bool managed) {
      GL.DeleteProgram(Id);

      base.Dispose(managed);
    }

    private static ShaderType ConvertShaderType(Surreal.Graphics.Materials.ShaderType shaderType) {
      switch (shaderType) {
        case Surreal.Graphics.Materials.ShaderType.Compute:  return ShaderType.ComputeShader;
        case Surreal.Graphics.Materials.ShaderType.Vertex:   return ShaderType.VertexShader;
        case Surreal.Graphics.Materials.ShaderType.Fragment: return ShaderType.FragmentShader;
        case Surreal.Graphics.Materials.ShaderType.Geometry: return ShaderType.GeometryShader;

        default:
          throw new ArgumentOutOfRangeException(nameof(shaderType), shaderType, null);
      }
    }

    private static VertexAttribPointerType ConvertVertexType(VertexType type) {
      switch (type) {
        case VertexType.UnsignedByte:  return VertexAttribPointerType.UnsignedByte;
        case VertexType.Byte:          return VertexAttribPointerType.Byte;
        case VertexType.Short:         return VertexAttribPointerType.Short;
        case VertexType.UnsignedShort: return VertexAttribPointerType.UnsignedShort;
        case VertexType.Int:           return VertexAttribPointerType.Int;
        case VertexType.UnsignedInt:   return VertexAttribPointerType.UnsignedInt;
        case VertexType.Float:         return VertexAttribPointerType.Float;
        case VertexType.Double:        return VertexAttribPointerType.Double;

        default:
          throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
    }
  }
}