using Surreal.Graphics.Cameras;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>An opaque handle to a resource in the underling <see cref="IGraphicsServer"/> implementation.</summary>
public readonly record struct GraphicsHandle(uint Id)
{
  public GraphicsHandle(int id)
    : this((uint) id)
  {
  }

  public static implicit operator uint(GraphicsHandle handle) => handle.Id;
  public static implicit operator int(GraphicsHandle handle) => (int) handle.Id;
}

/// <summary>An abstraction over the different types of graphics servers available.</summary>
public interface IGraphicsServer
{
  IShaderCompiler ShaderCompiler { get; }

  // intrinsics
  void SetViewportSize(Viewport viewport);
  void ClearColorBuffer(Color color);
  void ClearDepthBuffer();
  void FlushToDevice();

  // buffers
  GraphicsHandle CreateBuffer();
  Memory<T> ReadBufferData<T>(GraphicsHandle handle, Range range) where T : unmanaged;
  void WriteBufferData<T>(GraphicsHandle handle, ReadOnlySpan<T> data) where T : unmanaged;
  void DeleteBuffer(GraphicsHandle handle);

  // textures
  GraphicsHandle CreateTexture();
  Memory<T> ReadTextureData<T>(GraphicsHandle handle, int mipLevel = 0) where T : unmanaged;
  void WriteTextureData<T>(GraphicsHandle handle, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0) where T : unmanaged;
  void DeleteTexture(GraphicsHandle handle);

  // shaders
  GraphicsHandle CreateShader();
  void CompileShader(GraphicsHandle handle, ShaderDeclaration declaration);
  void CompileShader(GraphicsHandle handle, ICompiledShader compiled);
  void SetShaderUniform(GraphicsHandle handle, string name, int value);
  void SetShaderUniform(GraphicsHandle handle, string name, float value);
  void SetShaderUniform(GraphicsHandle handle, string name, Vector2I value);
  void SetShaderUniform(GraphicsHandle handle, string name, Vector3I value);
  void SetShaderUniform(GraphicsHandle handle, string name, Vector2 value);
  void SetShaderUniform(GraphicsHandle handle, string name, Vector3 value);
  void SetShaderUniform(GraphicsHandle handle, string name, Vector4 value);
  void SetShaderUniform(GraphicsHandle handle, string name, Quaternion value);
  void SetShaderUniform(GraphicsHandle handle, string name, in Matrix3x2 value);
  void SetShaderUniform(GraphicsHandle handle, string name, in Matrix4x4 value);
  void DeleteShader(GraphicsHandle handle);
}
