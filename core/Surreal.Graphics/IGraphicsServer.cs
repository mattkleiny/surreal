using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>An opaque handle to a resource in the underling <see cref="IGraphicsServer"/> implementation.</summary>
public readonly record struct GraphicsHandle(nint Id)
{
  public static implicit operator nint(GraphicsHandle handle) => handle.Id;
  public static implicit operator int(GraphicsHandle handle) => (int) handle.Id;
  public static implicit operator uint(GraphicsHandle handle) => (uint) handle.Id;
}

/// <summary>An abstraction over the different types of graphics servers available.</summary>
public interface IGraphicsServer
{
  // intrinsics
  void SetViewportSize(Viewport viewport);
  void ClearColorBuffer(Color color);
  void ClearDepthBuffer();
  void FlushToDevice();

  // buffers
  GraphicsHandle CreateBuffer(BufferType type);
  Memory<T> ReadBufferData<T>(GraphicsHandle handle, BufferType type, nint offset, int length) where T : unmanaged;
  void WriteBufferData<T>(GraphicsHandle handle, BufferType type, ReadOnlySpan<T> data, BufferUsage usage) where T : unmanaged;
  void WriteBufferSubData<T>(GraphicsHandle handle, BufferType type, nint offset, ReadOnlySpan<T> data) where T : unmanaged;
  void DeleteBuffer(GraphicsHandle handle);

  // textures
  GraphicsHandle CreateTexture(TextureFilterMode filterMode, TextureWrapMode wrapMode);
  Memory<T> ReadTextureData<T>(GraphicsHandle handle, int mipLevel = 0) where T : unmanaged;
  Memory<T> ReadTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, int width, int height, int mipLevel = 0) where T : unmanaged;
  void WriteTextureData<T>(GraphicsHandle handle, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0) where T : unmanaged;
  void WriteTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0) where T : unmanaged;
  void SetTextureFilterMode(GraphicsHandle handle, TextureFilterMode mode);
  void SetTextureWrapMode(GraphicsHandle handle, TextureWrapMode mode);
  void DeleteTexture(GraphicsHandle handle);

  // meshes
  GraphicsHandle CreateMesh();
  void DrawMesh(GraphicsHandle mesh, GraphicsHandle shader, GraphicsHandle vertices, GraphicsHandle indices, VertexDescriptorSet descriptors, int vertexCount, int indexCount, MeshType meshType, Type indexType);
  void DeleteMesh(GraphicsHandle handle);

  // shaders
  GraphicsHandle CreateShader();
  void CompileShader(GraphicsHandle handle, ShaderDeclaration declaration);
  void SetShaderUniform(GraphicsHandle handle, string name, int value);
  void SetShaderUniform(GraphicsHandle handle, string name, float value);
  void SetShaderUniform(GraphicsHandle handle, string name, Point2 value);
  void SetShaderUniform(GraphicsHandle handle, string name, Point3 value);
  void SetShaderUniform(GraphicsHandle handle, string name, Vector2 value);
  void SetShaderUniform(GraphicsHandle handle, string name, Vector3 value);
  void SetShaderUniform(GraphicsHandle handle, string name, Vector4 value);
  void SetShaderUniform(GraphicsHandle handle, string name, Quaternion value);
  void SetShaderUniform(GraphicsHandle handle, string name, in Matrix3x2 value);
  void SetShaderUniform(GraphicsHandle handle, string name, in Matrix4x4 value);
  void SetShaderTexture(GraphicsHandle handle, string name, GraphicsHandle texture, int samplerSlot);
  void DeleteShader(GraphicsHandle handle);
}
