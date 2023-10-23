using Surreal.Collections;
using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;
using Surreal.Maths;

namespace Surreal.Graphics;

/// <summary>
/// A viewport size for camera rendering.
/// </summary>
public readonly record struct Viewport(int X, int Y, uint Width, uint Height);

/// <summary>
/// An opaque handle to a resource in the underling <see cref="IGraphicsBackend" /> implementation.
/// </summary>
[ExcludeFromCodeCoverage]
public readonly record struct GraphicsHandle(nint Id)
{
  public static GraphicsHandle None => default;
  public static GraphicsHandle Invalid => new(-1);

  public GraphicsHandle(uint Id)
    : this((nint)Id)
  {
  }

  public GraphicsHandle(int Id)
    : this((nint)Id)
  {
  }

  public static implicit operator nint(GraphicsHandle handle) => handle.Id;
  public static implicit operator int(GraphicsHandle handle) => (int)handle.Id;
  public static implicit operator uint(GraphicsHandle handle) => (uint)handle.Id;
}

/// <summary>
/// An opaque handle to a render target in the underling <see cref="IGraphicsBackend" /> implementation.
/// </summary>
public readonly record struct FrameBufferHandle
{
  public static FrameBufferHandle None => default;

  public required GraphicsHandle FrameBuffer { get; init; }
  public readonly GraphicsHandle ColorAttachment { get; init; }
  public readonly GraphicsHandle DepthStencilAttachment { get; init; }
}

/// <summary>
/// An abstraction over the different types of graphics backends available.
/// </summary>
public interface IGraphicsBackend
{
  /// <summary>
  /// A no-op <see cref="IGraphicsBackend" /> for headless environments and testing.
  /// </summary>
  static IGraphicsBackend Headless { get; } = new HeadlessGraphicsBackend();

  // intrinsics
  Viewport GetViewportSize();
  void SetViewportSize(Viewport viewport);
  void SetBlendState(BlendState? state);
  void SetPolygonMode(PolygonMode mode);
  void ClearColorBuffer(Color color);
  void ClearDepthBuffer(float depth);
  void ClearStencilBuffer(int amount);
  void FlushToDevice();

  // buffers
  GraphicsHandle CreateBuffer();
  Memory<T> ReadBufferData<T>(GraphicsHandle handle, BufferType type, nint offset, int length) where T : unmanaged;
  void WriteBufferData<T>(GraphicsHandle handle, BufferType type, ReadOnlySpan<T> data, BufferUsage usage) where T : unmanaged;
  void WriteBufferSubData<T>(GraphicsHandle handle, BufferType type, nint offset, ReadOnlySpan<T> data) where T : unmanaged;
  void DeleteBuffer(GraphicsHandle handle);

  // textures
  GraphicsHandle CreateTexture(TextureFilterMode filterMode, TextureWrapMode wrapMode);
  Memory<T> ReadTextureData<T>(GraphicsHandle handle, int mipLevel = 0) where T : unmanaged;
  void ReadTextureData<T>(GraphicsHandle handle, Span<T> buffer, int mipLevel = 0) where T : unmanaged;
  Memory<T> ReadTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, int mipLevel = 0) where T : unmanaged;
  void ReadTextureSubData<T>(GraphicsHandle handle, Span<T> buffer, int offsetX, int offsetY, uint width, uint height, int mipLevel = 0) where T : unmanaged;
  void WriteTextureData<T>(GraphicsHandle handle, uint width, uint height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0) where T : unmanaged;
  void WriteTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, ReadOnlySpan<T> pixels, int mipLevel = 0) where T : unmanaged;

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
  void ResizeFrameBuffer(FrameBufferHandle handle, uint width, uint height);

  void BlitToBackBuffer(
    FrameBufferHandle handle,
    uint sourceWidth,
    uint sourceHeight,
    uint destWidth,
    uint destHeight,
    BlitMask mask,
    TextureFilterMode filterMode);

  void BlitToBackBuffer(FrameBufferHandle handle,
    Material material,
    MaterialProperty<TextureSampler> samplerProperty,
    Optional<TextureFilterMode> filterMode,
    Optional<TextureWrapMode> wrapMode);

  void DeleteFrameBuffer(FrameBufferHandle handle);
}
