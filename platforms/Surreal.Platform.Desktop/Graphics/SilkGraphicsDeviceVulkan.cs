using Surreal.Collections.Slices;
using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using PolygonMode = Surreal.Graphics.Materials.PolygonMode;
using TextureWrapMode = Surreal.Graphics.Textures.TextureWrapMode;

namespace Surreal.Graphics;

internal sealed class SilkGraphicsDeviceVulkan : IGraphicsDevice
{
  public Viewport GetViewportSize()
  {
    throw new NotImplementedException();
  }

  public void SetViewportSize(Viewport viewport)
  {
    throw new NotImplementedException();
  }

  public void SetBlendState(BlendState? state)
  {
    throw new NotImplementedException();
  }

  public void SetScissorState(ScissorState? state)
  {
    throw new NotImplementedException();
  }

  public void SetPolygonMode(PolygonMode mode)
  {
    throw new NotImplementedException();
  }

  public void SetCullingMode(CullingMode mode)
  {
    throw new NotImplementedException();
  }

  public void ClearColorBuffer(Color color)
  {
    throw new NotImplementedException();
  }

  public void ClearDepthBuffer(float depth)
  {
    throw new NotImplementedException();
  }

  public void ClearStencilBuffer(int amount)
  {
    throw new NotImplementedException();
  }

  public GraphicsHandle CreateBuffer()
  {
    throw new NotImplementedException();
  }

  public Memory<T> ReadBufferData<T>(GraphicsHandle handle, BufferType type, IntPtr offset, int length) where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void ReadBufferData<T>(GraphicsHandle handle, BufferType type, Span<T> buffer) where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void WriteBufferData<T>(GraphicsHandle handle, BufferType type, ReadOnlySpan<T> data, BufferUsage usage) where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void WriteBufferSubData<T>(GraphicsHandle handle, BufferType type, IntPtr offset, ReadOnlySpan<T> data) where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void DeleteBuffer(GraphicsHandle handle)
  {
    throw new NotImplementedException();
  }

  public GraphicsHandle CreateTexture(TextureFilterMode filterMode, TextureWrapMode wrapMode)
  {
    throw new NotImplementedException();
  }

  public Memory<T> ReadTextureData<T>(GraphicsHandle handle, int mipLevel = 0) where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void ReadTextureData<T>(GraphicsHandle handle, Span<T> buffer, int mipLevel = 0) where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public Memory<T> ReadTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, int mipLevel = 0) where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void ReadTextureSubData<T>(GraphicsHandle handle, Span<T> buffer, int offsetX, int offsetY, uint width, uint height, int mipLevel = 0) where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void WriteTextureData<T>(GraphicsHandle handle, uint width, uint height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0) where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void WriteTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, ReadOnlySpan<T> pixels, int mipLevel = 0) where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void SetTextureFilterMode(GraphicsHandle handle, TextureFilterMode mode)
  {
    throw new NotImplementedException();
  }

  public void SetTextureWrapMode(GraphicsHandle handle, TextureWrapMode mode)
  {
    throw new NotImplementedException();
  }

  public void DeleteTexture(GraphicsHandle handle)
  {
    throw new NotImplementedException();
  }

  public GraphicsHandle CreateMesh(GraphicsHandle vertices, GraphicsHandle indices, VertexDescriptorSet descriptors)
  {
    throw new NotImplementedException();
  }

  public void DrawMesh(GraphicsHandle mesh, uint vertexCount, uint indexCount, MeshType meshType, Type indexType)
  {
    throw new NotImplementedException();
  }

  public void DeleteMesh(GraphicsHandle handle)
  {
    throw new NotImplementedException();
  }

  public GraphicsHandle CreateShader()
  {
    throw new NotImplementedException();
  }

  public void LinkShader(GraphicsHandle handle, ReadOnlySlice<ShaderKernel> kernels)
  {
    throw new NotImplementedException();
  }

  public int GetShaderUniformLocation(GraphicsHandle handle, string name)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, int value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, float value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, double value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Point2 value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Point3 value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Point4 value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Vector2 value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Vector3 value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Vector4 value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Color value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Color32 value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Quaternion value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, in Matrix3x2 value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, in Matrix4x4 value)
  {
    throw new NotImplementedException();
  }

  public void SetShaderSampler(GraphicsHandle handle, int location, GraphicsHandle texture, uint samplerSlot)
  {
    throw new NotImplementedException();
  }

  public void SetShaderSampler(GraphicsHandle handle, int location, TextureSampler sampler)
  {
    throw new NotImplementedException();
  }

  public void SetActiveShader(GraphicsHandle handle)
  {
    throw new NotImplementedException();
  }

  public void DeleteShader(GraphicsHandle handle)
  {
    throw new NotImplementedException();
  }

  public FrameBufferHandle CreateFrameBuffer(RenderTargetDescriptor descriptor)
  {
    throw new NotImplementedException();
  }

  public bool IsActiveFrameBuffer(FrameBufferHandle handle)
  {
    throw new NotImplementedException();
  }

  public void BindFrameBuffer(FrameBufferHandle handle)
  {
    throw new NotImplementedException();
  }

  public void UnbindFrameBuffer()
  {
    throw new NotImplementedException();
  }

  public void ResizeFrameBuffer(FrameBufferHandle handle, uint width, uint height)
  {
    throw new NotImplementedException();
  }

  public void BlitFromFrameBuffer(GraphicsHandle targetFrameBuffer, uint sourceWidth, uint sourceHeight, uint destWidth, uint destHeight, BlitMask mask, TextureFilterMode filterMode)
  {
    throw new NotImplementedException();
  }

  public void BlitToFrameBuffer(GraphicsHandle sourceFrameBuffer, uint sourceWidth, uint sourceHeight, uint destWidth, uint destHeight, BlitMask mask, TextureFilterMode filterMode)
  {
    throw new NotImplementedException();
  }

  public void BlitToFrameBuffer(FrameBufferHandle sourceFrameBuffer, Material material, ShaderProperty<TextureSampler> samplerProperty, Optional<TextureFilterMode> filterMode, Optional<TextureWrapMode> wrapMode)
  {
    throw new NotImplementedException();
  }

  public void DeleteFrameBuffer(FrameBufferHandle handle)
  {
    throw new NotImplementedException();
  }

  public void BeginDebugScope(string name)
  {
    throw new NotImplementedException();
  }

  public void EndDebugScope()
  {
    throw new NotImplementedException();
  }

  public void Dispose()
  {
  }
}
