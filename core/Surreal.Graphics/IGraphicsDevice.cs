using Surreal.Collections.Slices;
using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>
/// A viewport size for camera rendering.
/// </summary>
public readonly record struct Viewport(int X, int Y, uint Width, uint Height);

/// <summary>
/// An opaque handle to a render target in the underling <see cref="IGraphicsBackend" /> implementation,
/// and it's accompanying color and depth/stencil attachments.
/// </summary>
public readonly record struct FrameBufferHandle(GraphicsHandle FrameBuffer)
{
  public static FrameBufferHandle None => default;

  public GraphicsHandle ColorAttachment { get; init; }
  public GraphicsHandle DepthStencilAttachment { get; init; }
}

/// <summary>
/// Represents a graphics device in the underlying operating system.
/// </summary>
public interface IGraphicsDevice : IDisposable
{
  static IGraphicsDevice Null { get; } = new NullGraphicsDevice();

  // intrinsics
  Viewport GetViewportSize();
  void SetViewportSize(Viewport viewport);
  void SetBlendState(BlendState? state);
  void SetScissorState(ScissorState? state);
  void SetPolygonMode(PolygonMode mode);
  void SetCullingMode(CullingMode mode);
  void ClearColorBuffer(Color color);
  void ClearDepthBuffer(float depth);
  void ClearStencilBuffer(int amount);

  // buffers
  GraphicsHandle CreateBuffer(BufferType type, BufferUsage usage);
  GraphicsTask<Memory<T>> ReadBufferDataAsync<T>(GraphicsHandle handle, BufferType type) where T : unmanaged;
  GraphicsTask WriteBufferDataAsync<T>(GraphicsHandle handle, BufferType type, ReadOnlySpan<T> span, BufferUsage usage) where T : unmanaged;
  GraphicsTask WriteBufferDataAsync<T>(GraphicsHandle handle, BufferType type, uint offset, ReadOnlySpan<T> span, BufferUsage usage) where T : unmanaged;
  void DeleteBuffer(GraphicsHandle handle);

  // textures
  GraphicsHandle CreateTexture(TextureFilterMode filterMode, TextureWrapMode wrapMode);
  GraphicsTask<Memory<T>> ReadTextureDataAsync<T>(GraphicsHandle handle, int mipLevel = 0) where T : unmanaged;
  GraphicsTask<Memory<T>> ReadTextureDataAsync<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, int mipLevel = 0) where T : unmanaged;
  GraphicsTask WriteTextureDataAsync<T>(GraphicsHandle handle, uint width, uint height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0) where T : unmanaged;
  GraphicsTask WriteTextureDataAsync<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, ReadOnlySpan<T> pixels, int mipLevel = 0) where T : unmanaged;
  void SetTextureFilterMode(GraphicsHandle handle, TextureFilterMode mode);
  void SetTextureWrapMode(GraphicsHandle handle, TextureWrapMode mode);
  void DeleteTexture(GraphicsHandle handle);

  // meshes
  GraphicsHandle CreateMesh(GraphicsHandle vertices, GraphicsHandle indices, VertexDescriptorSet descriptors);
  void DrawMesh(GraphicsHandle mesh, uint vertexCount, uint indexCount, MeshType meshType, Type indexType);
  void DeleteMesh(GraphicsHandle handle);

  // shaders
  GraphicsHandle CreateShader();
  void LinkShader(GraphicsHandle handle, ReadOnlySlice<ShaderKernel> kernels);
  int GetShaderUniformLocation(GraphicsHandle handle, string name);
  void SetShaderUniform(GraphicsHandle handle, int location, int value);
  void SetShaderUniform(GraphicsHandle handle, int location, float value);
  void SetShaderUniform(GraphicsHandle handle, int location, double value);
  void SetShaderUniform(GraphicsHandle handle, int location, Point2 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Point3 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Point4 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Vector2 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Vector3 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Vector4 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Color value);
  void SetShaderUniform(GraphicsHandle handle, int location, Color32 value);
  void SetShaderUniform(GraphicsHandle handle, int location, Quaternion value);
  void SetShaderUniform(GraphicsHandle handle, int location, in Matrix3x2 value);
  void SetShaderUniform(GraphicsHandle handle, int location, in Matrix4x4 value);
  void SetShaderSampler(GraphicsHandle handle, int location, GraphicsHandle texture, uint samplerSlot);
  void SetShaderSampler(GraphicsHandle handle, int location, TextureSampler sampler);
  void SetActiveShader(GraphicsHandle handle);
  void DeleteShader(GraphicsHandle handle);

  // frame buffers
  FrameBufferHandle CreateFrameBuffer(RenderTargetDescriptor descriptor);
  bool IsActiveFrameBuffer(FrameBufferHandle handle);
  void BindFrameBuffer(FrameBufferHandle handle);
  void UnbindFrameBuffer();
  void ResizeFrameBuffer(FrameBufferHandle handle, uint width, uint height);
  GraphicsTask BlitFromDisplayFrameBufferAsync(GraphicsHandle targetBuffer, uint sourceWidth, uint sourceHeight, uint destWidth, uint destHeight, BlitMask mask, TextureFilterMode filterMode);
  GraphicsTask BlitToTargetFrameBufferAsync(FrameBufferHandle sourceBuffer, FrameBufferHandle targetBuffer, Material material, ShaderProperty<TextureSampler> samplerProperty, Optional<TextureFilterMode> filterMode, Optional<TextureWrapMode> wrapMode);
  GraphicsTask BlitToDisplayFrameBufferAsync(GraphicsHandle sourceBuffer, uint sourceWidth, uint sourceHeight, uint destWidth, uint destHeight, BlitMask mask, TextureFilterMode filterMode);
  GraphicsTask BlitToDisplayFrameBufferAsync(FrameBufferHandle sourceBuffer, Material material, ShaderProperty<TextureSampler> samplerProperty, Optional<TextureFilterMode> filterMode, Optional<TextureWrapMode> wrapMode);
  void DeleteFrameBuffer(FrameBufferHandle handle);

  // debugging
  void BeginDebugScope(string name);
  void EndDebugScope();

  /// <summary>
  /// A no-op <see cref="IGraphicsDevice" /> for headless environments and testing.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullGraphicsDevice : IGraphicsDevice
  {
    private Viewport _viewportSize = new(0, 0, 1920, 1080);
    private int _nextBufferId;
    private int _nextMeshId;
    private int _nextShaderId;
    private int _nextTextureId;
    private int _nextFrameBufferId;
    private FrameBufferHandle _activeFrameBuffer;

    public Viewport GetViewportSize()
    {
      return _viewportSize;
    }

    public void SetViewportSize(Viewport viewport)
    {
      _viewportSize = viewport;
    }

    public void SetBlendState(BlendState? state)
    {
    }

    public void SetScissorState(ScissorState? state)
    {
    }

    public void SetPolygonMode(PolygonMode mode)
    {
    }

    public void SetCullingMode(CullingMode mode)
    {
    }

    public void ClearColorBuffer(Color color)
    {
    }

    public void ClearDepthBuffer(float depth)
    {
    }

    public void ClearStencilBuffer(int amount)
    {
    }

    public GraphicsHandle CreateBuffer(BufferType type, BufferUsage usage)
    {
      return GraphicsHandle.FromInt(Interlocked.Increment(ref _nextBufferId));
    }

    public GraphicsTask<Memory<T>> ReadBufferDataAsync<T>(GraphicsHandle handle, BufferType type) where T : unmanaged
    {
      return GraphicsTask.FromResult(Memory<T>.Empty);
    }

    public GraphicsTask WriteBufferDataAsync<T>(GraphicsHandle handle, BufferType type, uint offset, ReadOnlySpan<T> span, BufferUsage usage) where T : unmanaged
    {
      return GraphicsTask.CompletedTask;
    }

    public void DeleteBuffer(GraphicsHandle handle)
    {
    }

    public GraphicsTask WriteBufferDataAsync<T>(GraphicsHandle handle, BufferType type, ReadOnlySpan<T> span, BufferUsage usage)
      where T : unmanaged
    {
      return GraphicsTask.CompletedTask;
    }

    public GraphicsHandle CreateTexture(TextureFilterMode filterMode, TextureWrapMode wrapMode)
    {
      return GraphicsHandle.FromInt(Interlocked.Increment(ref _nextTextureId));
    }

    public GraphicsTask<Memory<T>> ReadTextureDataAsync<T>(GraphicsHandle handle, int mipLevel = 0)
      where T : unmanaged
    {
      return GraphicsTask.FromResult(Memory<T>.Empty);
    }

    public GraphicsTask<Memory<T>> ReadTextureDataAsync<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, int mipLevel = 0)
      where T : unmanaged
    {
      return GraphicsTask.FromResult(Memory<T>.Empty);
    }

    public GraphicsTask WriteTextureDataAsync<T>(GraphicsHandle handle, uint width, uint height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0) where T : unmanaged
    {
      return GraphicsTask.CompletedTask;
    }

    public GraphicsTask WriteTextureDataAsync<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, ReadOnlySpan<T> pixels, int mipLevel = 0)
      where T : unmanaged
    {
      return GraphicsTask.CompletedTask;
    }

    public void SetTextureFilterMode(GraphicsHandle handle, TextureFilterMode mode)
    {
    }

    public void SetTextureWrapMode(GraphicsHandle handle, TextureWrapMode mode)
    {
    }

    public void DeleteTexture(GraphicsHandle handle)
    {
    }

    public GraphicsHandle CreateMesh(GraphicsHandle vertices, GraphicsHandle indices, VertexDescriptorSet descriptors)
    {
      return GraphicsHandle.FromInt(Interlocked.Increment(ref _nextMeshId));
    }

    public void DrawMesh(GraphicsHandle mesh, uint vertexCount, uint indexCount, MeshType meshType, Type indexType)
    {
    }

    public void DeleteMesh(GraphicsHandle handle)
    {
    }

    public GraphicsHandle CreateShader()
    {
      return GraphicsHandle.FromInt(Interlocked.Increment(ref _nextShaderId));
    }

    public void LinkShader(GraphicsHandle handle, ReadOnlySlice<ShaderKernel> kernels)
    {
    }

    public int GetShaderUniformLocation(GraphicsHandle handle, string name)
    {
      return 0;
    }

    public void SetShaderSampler(GraphicsHandle handle, int location, TextureSampler sampler)
    {
    }

    public void SetActiveShader(GraphicsHandle handle)
    {
    }

    public void DeleteShader(GraphicsHandle handle)
    {
    }

    public FrameBufferHandle CreateFrameBuffer(RenderTargetDescriptor descriptor)
    {
      return new FrameBufferHandle(GraphicsHandle.FromInt(Interlocked.Increment(ref _nextFrameBufferId)));
    }

    public bool IsActiveFrameBuffer(FrameBufferHandle handle)
    {
      return handle == _activeFrameBuffer;
    }

    public void BindFrameBuffer(FrameBufferHandle handle)
    {
      _activeFrameBuffer = handle;
    }

    public void UnbindFrameBuffer()
    {
      _activeFrameBuffer = default;
    }

    public void ResizeFrameBuffer(FrameBufferHandle handle, uint width, uint height)
    {
    }

    public GraphicsTask BlitFromDisplayFrameBufferAsync(GraphicsHandle targetBuffer, uint sourceWidth, uint sourceHeight, uint destWidth, uint destHeight, BlitMask mask, TextureFilterMode filterMode)
    {
      return GraphicsTask.CompletedTask;
    }

    public GraphicsTask BlitToTargetFrameBufferAsync(FrameBufferHandle sourceBuffer, FrameBufferHandle targetBuffer, Material material, ShaderProperty<TextureSampler> samplerProperty, Optional<TextureFilterMode> filterMode, Optional<TextureWrapMode> wrapMode)
    {
      return GraphicsTask.CompletedTask;
    }

    public GraphicsTask BlitToDisplayFrameBufferAsync(GraphicsHandle sourceBuffer, uint sourceWidth, uint sourceHeight, uint destWidth, uint destHeight, BlitMask mask, TextureFilterMode filterMode)
    {
      return GraphicsTask.CompletedTask;
    }

    public GraphicsTask BlitToDisplayFrameBufferAsync(FrameBufferHandle sourceBuffer,
      Material material,
      ShaderProperty<TextureSampler> samplerProperty,
      Optional<TextureFilterMode> filterMode,
      Optional<TextureWrapMode> wrapMode)
    {
      return GraphicsTask.CompletedTask;
    }

    public void DeleteFrameBuffer(FrameBufferHandle handle)
    {
    }

    public void BeginDebugScope(string name)
    {
    }

    public void EndDebugScope()
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, int value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, float value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, double value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, Point2 value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, Point3 value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, Point4 value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, Vector2 value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, Vector3 value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, Vector4 value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, Color value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, Color32 value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, Quaternion value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, in Matrix3x2 value)
    {
    }

    public void SetShaderUniform(GraphicsHandle handle, int location, in Matrix4x4 value)
    {
    }

    public void SetShaderSampler(GraphicsHandle handle, int location, GraphicsHandle texture, uint samplerSlot)
    {
    }

    public void Dispose()
    {
    }
  }
}
