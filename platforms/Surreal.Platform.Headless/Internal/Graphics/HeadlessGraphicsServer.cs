using Surreal.Graphics;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Internal.Graphics;

internal sealed class HeadlessGraphicsServer : IGraphicsServer
{
  private int nextBufferId  = 0;
  private int nextTextureId = 0;
  private int nextShaderId  = 0;

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

  public GraphicsHandle CreateBuffer()
  {
    return new GraphicsHandle(Interlocked.Increment(ref nextBufferId));
  }

  public void DeleteBuffer(GraphicsHandle handle)
  {
    // no-op
  }

  public Memory<T> ReadBufferData<T>(GraphicsHandle handle, Range range) where T : unmanaged
  {
    return Memory<T>.Empty;
  }

  public void WriteBufferData<T>(GraphicsHandle handle, ReadOnlySpan<T> data) where T : unmanaged
  {
    // no-op
  }

  public GraphicsHandle CreateTexture()
  {
    return new GraphicsHandle(Interlocked.Increment(ref nextTextureId));
  }

  public void DeleteTexture(GraphicsHandle handle)
  {
    // no-op
  }

  public Memory<T> ReadTextureData<T>(GraphicsHandle handle, int mipLevel = 0) where T : unmanaged
  {
    return Memory<T>.Empty;
  }

  public void WriteTextureData<T>(GraphicsHandle handle, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0) where T : unmanaged
  {
    // no-op
  }

  public GraphicsHandle CreateShader()
  {
    return new GraphicsHandle(Interlocked.Increment(ref nextShaderId));
  }

  public void DeleteShader(GraphicsHandle handle)
  {
    // no-op
  }

  public void CompileShader(GraphicsHandle handle, ShaderDeclaration declaration)
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

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector2I value)
  {
    // no-op
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector3I value)
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
