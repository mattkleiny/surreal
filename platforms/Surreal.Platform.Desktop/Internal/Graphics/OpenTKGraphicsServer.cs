using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Internal.Graphics.Resources;

namespace Surreal.Internal.Graphics;

/// <summary>The <see cref="IGraphicsServer"/> for the OpenTK backend (OpenGL).</summary>
internal sealed class OpenTKGraphicsServer : IGraphicsServer,
  IGraphicsServer.IBuffers,
  IGraphicsServer.ITextures,
  IGraphicsServer.IShaders,
  IGraphicsServer.IMaterials,
  IGraphicsServer.ILighting
{
  private readonly IShaderCompiler compiler;

  public OpenTKGraphicsServer()
    : this(new OpenTKShaderCompiler())
  {
  }

  public OpenTKGraphicsServer(IShaderCompiler compiler)
  {
    this.compiler = compiler;
  }

  public IGraphicsServer.IBuffers   Buffers   => this;
  public IGraphicsServer.ITextures  Textures  => this;
  public IGraphicsServer.IShaders   Shaders   => this;
  public IGraphicsServer.IMaterials Materials => this;
  public IGraphicsServer.ILighting  Lighting  => this;

  public GraphicsId CreateBuffer()
  {
    var buffer = GL.GenBuffer();

    return new GraphicsId(buffer.Handle);
  }

  public unsafe void UploadBufferData<T>(GraphicsId id, ReadOnlySpan<T> data) where T : unmanaged
  {
    var buffer = new BufferHandle(id);
    var bytes  = data.Length * sizeof(T);

    fixed (T* pointer = data)
    {
      GL.BindBuffer(BufferTargetARB.ArrayBuffer, buffer);
      GL.BufferData(BufferTargetARB.ArrayBuffer, bytes, pointer, BufferUsageARB.DynamicCopy);
    }
  }

  public GraphicsId CreateTexture()
  {
    var texture = GL.GenTexture();

    return new GraphicsId(texture.Handle);
  }

  public void AllocateTexture(GraphicsId id, int width, int height, int depth, TextureFormat format)
  {
    var texture = new TextureHandle(id);

    GL.BindTexture(TextureTarget.Texture2d, texture);
    GL.PixelStorei(PixelStoreParameter.UnpackAlignment, 1);

    throw new NotImplementedException();
  }

  public unsafe void UploadTextureData<T>(GraphicsId id, int width, int height, ReadOnlySpan<T> data, int mipLevel) where T : unmanaged
  {
    var texture = new TextureHandle(id);

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

  public GraphicsId CreateShader()
  {
    var shader = GL.CreateProgram();

    return new GraphicsId(shader.Handle);
  }

  public void CompileShader(GraphicsId id, ShaderDeclaration declaration)
  {
    var program = new ProgramHandle(id);

    var (_, shaders) = (OpenTKShaderSet) compiler.Compile(declaration);
    var shaderIds = new ShaderHandle[shaders.Length];

    GL.UseProgram(program);

    for (var i = 0; i < shaders.Length; i++)
    {
      var (shaderType, code) = shaders[i];

      var shader = shaderIds[i] = GL.CreateShader(shaderType);

      GL.ShaderSource(shader, code);
      GL.CompileShader(shader);

      var compileStatus = 0;

      GL.GetShaderi(shader, ShaderParameterName.CompileStatus, ref compileStatus);

      if (compileStatus != 1)
      {
        GL.GetShaderInfoLog(shader, out var errorLog);

        throw new ShaderProgramException($"An error occurred whilst compiling a {shaderType} shader.", errorLog);
      }

      GL.AttachShader(program, shader);
    }

    int linkStatus = 0;

    GL.LinkProgram(program);
    GL.GetProgrami(program, ProgramPropertyARB.LinkStatus, ref linkStatus);

    if (linkStatus != 1)
    {
      GL.GetProgramInfoLog(program, out var errorLog);

      throw new ShaderProgramException("An error occurred whilst linking a shader program.", errorLog);
    }

    foreach (var shaderId in shaderIds)
    {
      GL.DeleteShader(shaderId);
    }
  }

  public GraphicsId CreateMaterial()
  {
    throw new NotImplementedException();
  }

  public void SetMaterialShader(GraphicsId materialId, GraphicsId shaderId)
  {
    throw new NotImplementedException();
  }

  public GraphicsId CreateLight()
  {
    throw new NotImplementedException();
  }

  public void SetLightTransform(GraphicsId id, in Matrix4x4 transform)
  {
    throw new NotImplementedException();
  }

  public void SetShadowTransform(GraphicsId id, in Matrix4x4 transform)
  {
    throw new NotImplementedException();
  }
}
