using System.Runtime.CompilerServices;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Internal.Graphics;

/// <summary>The <see cref="IGraphicsServer"/> for the OpenTK backend (OpenGL).</summary>
internal sealed class OpenTKGraphicsServer : IGraphicsServer
{
  private readonly OpenTKShaderCompiler shaderCompiler = new();

  public GraphicsHandle CreateBuffer()
  {
    var buffer = GL.GenBuffer();

    return new GraphicsHandle(buffer.Handle);
  }

  public void DeleteBuffer(GraphicsHandle handle)
  {
    var buffer = new BufferHandle(handle);

    GL.DeleteBuffer(buffer);
  }

  public unsafe void UploadBufferData<T>(GraphicsHandle handle, ReadOnlySpan<T> data) where T : unmanaged
  {
    var buffer = new BufferHandle(handle);
    var bytes  = data.Length * sizeof(T);

    fixed (T* pointer = data)
    {
      GL.BindBuffer(BufferTargetARB.ArrayBuffer, buffer);
      GL.BufferData(BufferTargetARB.ArrayBuffer, bytes, pointer, BufferUsageARB.DynamicCopy);
    }
  }

  public GraphicsHandle CreateTexture()
  {
    var texture = GL.GenTexture();

    return new GraphicsHandle(texture.Handle);
  }

  public void DeleteTexture(GraphicsHandle handle)
  {
    var texture = new TextureHandle(handle);

    GL.DeleteTexture(texture);
  }

  public void AllocateTexture(GraphicsHandle handle, int width, int height, int depth, TextureFormat format)
  {
    var texture = new TextureHandle(handle);

    GL.BindTexture(TextureTarget.Texture2d, texture);
    GL.PixelStorei(PixelStoreParameter.UnpackAlignment, 1);

    throw new NotImplementedException();
  }

  public unsafe void UploadTextureData<T>(GraphicsHandle handle, int width, int height, ReadOnlySpan<T> data, int mipLevel) where T : unmanaged
  {
    var texture = new TextureHandle(handle);

    GL.BindTexture(TextureTarget.Texture2d, texture);

    fixed (T* pointer = data)
    {
      GL.TexImage2D(
        target: TextureTarget.Texture2d,
        level: mipLevel,
        internalformat: 0, // TODO: fix me
        width: width,
        height: height,
        border: 0,
        format: PixelFormat.Rgba,
        type: PixelType.Byte,
        pixels: pointer
      );
    }
  }

  public GraphicsHandle CreateShader()
  {
    var shader = GL.CreateProgram();

    return new GraphicsHandle(shader.Handle);
  }

  public void CompileShader(GraphicsHandle handle, ShaderDeclaration declaration)
  {
    var program = new ProgramHandle(handle);

    var (path, shaders) = shaderCompiler.Compile(declaration);
    var shaderIds = new ShaderHandle[shaders.Length];

    GL.UseProgram(program);

    for (var i = 0; i < shaders.Length; i++)
    {
      var (stage, code) = shaders[i];

      var shader = shaderIds[i] = GL.CreateShader(stage);

      GL.ShaderSource(shader, code);
      GL.CompileShader(shader);

      var compileStatus = 0;

      GL.GetShaderi(shader, ShaderParameterName.CompileStatus, ref compileStatus);

      if (compileStatus != 1)
      {
        GL.GetShaderInfoLog(shader, out var errorLog);
        GL.DeleteShader(shader); // don't leak the shader

        throw new PlatformException($"An error occurred whilst compiling a {stage} shader from {path}: {errorLog}");
      }

      GL.AttachShader(program, shader);
    }

    int linkStatus = 0;

    GL.LinkProgram(program);
    GL.GetProgrami(program, ProgramPropertyARB.LinkStatus, ref linkStatus);

    if (linkStatus != 1)
    {
      GL.GetProgramInfoLog(program, out var errorLog);
      GL.DeleteProgram(program); // don't leak the program

      throw new PlatformException($"An error occurred whilst linking a shader program from {path}: {errorLog}");
    }

    // we're finished with the shaders, now
    foreach (var shaderId in shaderIds)
    {
      GL.DeleteShader(shaderId);
    }
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, int value)
  {
    var program  = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);

    GL.Uniform1i(location, value);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, float value)
  {
    var program  = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);

    GL.Uniform1f(location, value);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector2I value)
  {
    var program  = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);

    GL.Uniform2i(location, value.X, value.Y);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector3I value)
  {
    var program  = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);

    GL.Uniform3i(location, value.X, value.Y, value.Z);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector2 value)
  {
    var program  = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);

    GL.Uniform2f(location, value.X, value.Y);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector3 value)
  {
    var program  = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);

    GL.Uniform3f(location, value.X, value.Y, value.Z);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector4 value)
  {
    var program  = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);

    GL.Uniform4f(location, value.X, value.Y, value.Z, value.W);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Quaternion value)
  {
    var program  = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);

    GL.Uniform4f(location, value.X, value.Y, value.Z, value.W);
  }

  public unsafe void SetShaderUniform(GraphicsHandle handle, string name, in Matrix3x2 value)
  {
    var program  = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);

    var pointer = (float*) Unsafe.AsPointer(ref Unsafe.AsRef(in value));

    GL.UniformMatrix4f(location, 1, false, new ReadOnlySpan<float>(pointer, 3 * 2));
  }

  public unsafe void SetShaderUniform(GraphicsHandle handle, string name, in Matrix4x4 value)
  {
    var program  = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);

    var pointer = (float*) Unsafe.AsPointer(ref Unsafe.AsRef(in value));

    GL.UniformMatrix4f(location, 1, false, new ReadOnlySpan<float>(pointer, 4 * 4));
  }

  public void DeleteShader(GraphicsHandle handle)
  {
    var program = new ProgramHandle(handle);

    GL.DeleteProgram(program);
  }
}
