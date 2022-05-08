using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>A no-op <see cref="IGraphicsServer"/> for headless environments and testing.</summary>
public sealed class HeadlessGraphicsServer : IGraphicsServer
{
  private int nextBufferId = 0;
  private int nextTextureId = 0;
  private int nextMeshId = 0;
  private int nextShaderId = 0;

  public void SetViewportSize(Viewport viewport)
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
    return new GraphicsHandle(Interlocked.Increment(ref nextBufferId));
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
    return new GraphicsHandle(Interlocked.Increment(ref nextTextureId));
  }

  public Memory<T> ReadTextureData<T>(GraphicsHandle handle, int mipLevel = 0) where T : unmanaged
  {
    return Memory<T>.Empty;
  }

  public Memory<T> ReadTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, int width, int height, int mipLevel = 0) where T : unmanaged
  {
    return Memory<T>.Empty;
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

  public GraphicsHandle CreateMesh()
  {
    return new GraphicsHandle(Interlocked.Increment(ref nextMeshId));
  }

  public void DrawMesh(GraphicsHandle mesh, GraphicsHandle shader, GraphicsHandle vertices, GraphicsHandle indices, VertexDescriptorSet descriptors, int vertexCount, int indexCount, MeshType meshType, Type indexType)
  {
    // no-op
  }

  public void DeleteMesh(GraphicsHandle handle)
  {
    // no-op
  }

  public GraphicsHandle CreateShader()
  {
    return new GraphicsHandle(Interlocked.Increment(ref nextShaderId));
  }

  public void CompileShader(GraphicsHandle handle, ShaderDeclaration declaration)
  {
    // no-op
  }

  public void SetShaderTexture(GraphicsHandle handle, string name, GraphicsHandle texture, int samplerSlot)
  {
    // no-op
  }

  public void DeleteShader(GraphicsHandle handle)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, int value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, float value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Point2 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Point3 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector2 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector3 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector4 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Quaternion value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, in Matrix3x2 value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, in Matrix4x4 value)
  {
    // no-op
  }
}
