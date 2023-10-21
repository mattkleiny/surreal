using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Maths;

namespace Surreal.Graphics;

/// <summary>
/// An abstraction over the different types of graphics backends available.
/// </summary>
public interface IGraphicsBackend
{
  // intrinsics
  void SetViewportSize(Viewport viewport);
  void SetBlendState(BlendState state);
  void ClearColorBuffer(Color color);
  void ClearDepthBuffer();
  void FlushToDevice();

  // buffers
  GraphicsHandle CreateBuffer(BufferType type);

  Memory<T> ReadBufferData<T>(GraphicsHandle handle, BufferType type, nint offset, int length)
    where T : unmanaged;

  void WriteBufferData<T>(GraphicsHandle handle, BufferType type, ReadOnlySpan<T> data, BufferUsage usage)
    where T : unmanaged;

  void WriteBufferSubData<T>(GraphicsHandle handle, BufferType type, nint offset, ReadOnlySpan<T> data)
    where T : unmanaged;

  void DeleteBuffer(GraphicsHandle handle);

  // textures
  GraphicsHandle CreateTexture(TextureFilterMode filterMode, TextureWrapMode wrapMode);

  Memory<T> ReadTextureData<T>(GraphicsHandle handle, int mipLevel = 0)
    where T : unmanaged;

  void ReadTextureData<T>(GraphicsHandle handle, Span<T> buffer, int mipLevel = 0)
    where T : unmanaged;

  Memory<T> ReadTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, int width, int height, int mipLevel = 0)
    where T : unmanaged;

  void ReadTextureSubData<T>(GraphicsHandle handle, Span<T> buffer, int offsetX, int offsetY, int width, int height, int mipLevel = 0)
    where T : unmanaged;

  void WriteTextureData<T>(GraphicsHandle handle, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0)
    where T : unmanaged;

  void WriteTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0)
    where T : unmanaged;

  void SetTextureFilterMode(GraphicsHandle handle, TextureFilterMode mode);
  void SetTextureWrapMode(GraphicsHandle handle, TextureWrapMode mode);
  void DeleteTexture(GraphicsHandle handle);

  // meshes
  GraphicsHandle CreateMesh(GraphicsHandle vertices, GraphicsHandle indices, VertexDescriptorSet descriptors);
  void DrawMesh(GraphicsHandle mesh, uint vertexCount, uint indexCount, MeshType meshType, Type indexType);
  void DeleteMesh(GraphicsHandle handle);

  // shaders
  GraphicsHandle CreateShader();
  int GetShaderUniformLocation(GraphicsHandle handle, string name);
  void SetShaderUniform(GraphicsHandle handle, int location, int value);
  void SetShaderUniform(GraphicsHandle handle, int location, float value);
  void SetShaderUniform(GraphicsHandle handle, int location, Point2 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Point3 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Point4 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Vector2 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Vector3 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Vector4 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Quaternion value);
  void SetShaderUniform(GraphicsHandle handle, int location, in Matrix3x2 value);
  void SetShaderUniform(GraphicsHandle handle, int location, in Matrix4x4 value);
  void SetShaderSampler(GraphicsHandle handle, int location, GraphicsHandle texture, int samplerSlot);
  void SetActiveShader(GraphicsHandle handle);
  void DeleteShader(GraphicsHandle handle);

  // frame buffers
}
