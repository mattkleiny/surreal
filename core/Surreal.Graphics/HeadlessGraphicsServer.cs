using Surreal.Collections;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>
/// A no-op <see cref="IGraphicsServer" /> for headless environments and testing.
/// </summary>
public sealed class HeadlessGraphicsServer : IGraphicsServer
{
  private int _nextBufferId;
  private int _nextFrameBufferId;
  private int _nextMeshId;
  private int _nextShaderId;
  private int _nextTextureId;

  public void SetViewportSize(Viewport viewport)
  {
    // no-op
  }

  public void SetBlendState(BlendState state)
  {
    // no-op
  }

  public void ClearColorBuffer(Color color)
  {
    // no-op
  }

  public void ClearDepthBuffer()
  {
    // no-op
  }

  public void FlushToDevice()
  {
    // no-op
  }

  public GraphicsHandle CreateBuffer(BufferType type)
  {
    return new GraphicsHandle(Interlocked.Increment(ref _nextBufferId));
  }

  public void DeleteBuffer(GraphicsHandle handle)
  {
    // no-op
  }

  public Memory<T> ReadBufferData<T>(GraphicsHandle handle, BufferType type, nint offset, int length) where T : unmanaged
  {
    return Memory<T>.Empty;
  }

  public void WriteBufferData<T>(GraphicsHandle handle, BufferType type, ReadOnlySpan<T> data, BufferUsage usage) where T : unmanaged
  {
    // no-op
  }

  public void WriteBufferSubData<T>(GraphicsHandle handle, BufferType type, nint offset, ReadOnlySpan<T> data) where T : unmanaged
  {
    // no-op
  }

  public GraphicsHandle CreateTexture(TextureFilterMode filterMode, TextureWrapMode wrapMode)
  {
    return new GraphicsHandle(Interlocked.Increment(ref _nextTextureId));
  }

  public Memory<T> ReadTextureData<T>(GraphicsHandle handle, int mipLevel = 0) where T : unmanaged
  {
    return Memory<T>.Empty;
  }

  public void ReadTextureData<T>(GraphicsHandle handle, Span<T> buffer, int mipLevel = 0) where T : unmanaged
  {
    // no-op
  }

  public Memory<T> ReadTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, int width, int height, int mipLevel = 0) where T : unmanaged
  {
    return Memory<T>.Empty;
  }

  public void ReadTextureSubData<T>(GraphicsHandle handle, Span<T> buffer, int offsetX, int offsetY, int width, int height, int mipLevel = 0) where T : unmanaged
  {
    // no-op
  }

  public void WriteTextureData<T>(GraphicsHandle handle, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0) where T : unmanaged
  {
    // no-op
  }

  public void WriteTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0) where T : unmanaged
  {
    // no-op
  }

  public void SetTextureFilterMode(GraphicsHandle handle, TextureFilterMode mode)
  {
    // no-op
  }

  public void SetTextureWrapMode(GraphicsHandle handle, TextureWrapMode mode)
  {
    // no-op
  }

  public void DeleteTexture(GraphicsHandle handle)
  {
    // no-op
  }

  public GraphicsHandle CreateMesh(GraphicsHandle vertices, GraphicsHandle indices, VertexDescriptorSet descriptors)
  {
    return new GraphicsHandle(Interlocked.Increment(ref _nextMeshId));
  }

  public void DrawMesh(GraphicsHandle mesh, int vertexCount, int indexCount, MeshType meshType, Type indexType)
  {
    // no-op
  }

  public void DeleteMesh(GraphicsHandle handle)
  {
    // no-op
  }

  public GraphicsHandle CreateShader()
  {
    return new GraphicsHandle(Interlocked.Increment(ref _nextShaderId));
  }

  public ReadOnlySlice<AttributeMetadata> GetShaderAttributeMetadata(GraphicsHandle handle)
  {
    return ReadOnlySlice<AttributeMetadata>.Empty;
  }

  public ReadOnlySlice<UniformMetadata> GetShaderUniformMetadata(GraphicsHandle handle)
  {
    return ReadOnlySlice<UniformMetadata>.Empty;
  }

  public int GetShaderUniformLocation(GraphicsHandle handle, string name)
  {
    return 0;
  }

  public void SetActiveShader(GraphicsHandle handle)
  {
    // no-op
  }

  public void DeleteShader(GraphicsHandle handle)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, int value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, float value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Point2 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Point3 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Point4 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Vector2 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Vector3 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Vector4 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Quaternion value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, in Matrix3x2 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, in Matrix4x4 value)
  {
    // no-op
  }

  public void SetShaderSampler(GraphicsHandle handle, int location, GraphicsHandle texture, int samplerSlot)
  {
    // no-op
  }

  public GraphicsHandle CreateFrameBuffer(GraphicsHandle colorAttachment)
  {
    return new GraphicsHandle(Interlocked.Increment(ref _nextFrameBufferId));
  }

  public void SetActiveFrameBuffer(GraphicsHandle handle)
  {
    // no-op
  }

  public void SetDefaultFrameBuffer()
  {
    // no-op
  }

  public void DeleteFrameBuffer(GraphicsHandle handle)
  {
    // no-op
  }
}
