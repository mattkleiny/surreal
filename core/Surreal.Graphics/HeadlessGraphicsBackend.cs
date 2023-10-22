﻿using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;
using Surreal.Maths;

namespace Surreal.Graphics;

/// <summary>
/// A no-op <see cref="IGraphicsBackend" /> for headless environments and testing.
/// </summary>
internal sealed class HeadlessGraphicsBackend : IGraphicsBackend
{
  private int _nextBufferId;
  private int _nextMeshId;
  private int _nextShaderId;
  private int _nextTextureId;
  private int _nextFrameBufferId;
  private FrameBufferHandle _activeFrameBuffer;

  public void SetViewportSize(Viewport viewport)
  {
  }

  public void SetBlendState(BlendState? state)
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

  public void FlushToDevice()
  {
  }

  public GraphicsHandle CreateBuffer(BufferType type)
  {
    return new GraphicsHandle(Interlocked.Increment(ref _nextBufferId));
  }

  public void DeleteBuffer(GraphicsHandle handle)
  {
  }

  public Memory<T> ReadBufferData<T>(GraphicsHandle handle, BufferType type, nint offset, int length)
    where T : unmanaged
  {
    return Memory<T>.Empty;
  }

  public void WriteBufferData<T>(GraphicsHandle handle, BufferType type, ReadOnlySpan<T> data, BufferUsage usage)
    where T : unmanaged
  {
  }

  public void WriteBufferSubData<T>(GraphicsHandle handle, BufferType type, nint offset, ReadOnlySpan<T> data)
    where T : unmanaged
  {
  }

  public GraphicsHandle CreateTexture(TextureFilterMode filterMode, TextureWrapMode wrapMode)
  {
    return new GraphicsHandle(Interlocked.Increment(ref _nextTextureId));
  }

  public Memory<T> ReadTextureData<T>(GraphicsHandle handle, int mipLevel = 0)
    where T : unmanaged
  {
    return Memory<T>.Empty;
  }

  public void ReadTextureData<T>(GraphicsHandle handle, Span<T> buffer, int mipLevel = 0)
    where T : unmanaged
  {
  }

  public Memory<T> ReadTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, int mipLevel = 0)
    where T : unmanaged
  {
    return Memory<T>.Empty;
  }

  public void ReadTextureSubData<T>(GraphicsHandle handle, Span<T> buffer, int offsetX, int offsetY, uint width, uint height, int mipLevel = 0)
    where T : unmanaged
  {
  }

  public void WriteTextureData<T>(GraphicsHandle handle, uint width, uint height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0)
    where T : unmanaged
  {
  }

  public void WriteTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0)
    where T : unmanaged
  {
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
    return new GraphicsHandle(Interlocked.Increment(ref _nextMeshId));
  }

  public void DrawMesh(GraphicsHandle mesh, uint vertexCount, uint indexCount, MeshType meshType, Type indexType)
  {
  }

  public void DeleteMesh(GraphicsHandle handle)
  {
  }

  public GraphicsHandle CreateShader()
  {
    return new GraphicsHandle(Interlocked.Increment(ref _nextShaderId));
  }

  public int GetShaderUniformLocation(GraphicsHandle handle, string name)
  {
    return 0;
  }

  public void SetActiveShader(GraphicsHandle handle)
  {
  }

  public void DeleteShader(GraphicsHandle handle)
  {
  }

  public FrameBufferHandle CreateFrameBuffer(RenderTargetDescriptor descriptor)
  {
    return new FrameBufferHandle
    {
      FrameBuffer = new GraphicsHandle(Interlocked.Increment(ref _nextFrameBufferId))
    };
  }

  public bool IsActiveFrameBuffer(FrameBufferHandle handle)
  {
    return handle == _activeFrameBuffer;
  }

  public void BindFrameBuffer(FrameBufferHandle handle)
  {
    _activeFrameBuffer = handle;
  }

  public void DeleteFrameBuffer(FrameBufferHandle handle)
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

  public void SetShaderSampler(GraphicsHandle handle, int location, GraphicsHandle texture, int samplerSlot)
  {
  }

  public void Dispose()
  {
  }
}
