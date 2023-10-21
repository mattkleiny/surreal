using Silk.NET.OpenGL;
using Surreal.Collections;
using Surreal.Colors;
using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Maths;
using TextureWrapMode = Surreal.Graphics.Textures.TextureWrapMode;

namespace Surreal.Internal.Graphics;

internal sealed class SilkGraphicsBackend(GL gl) : IGraphicsBackend
{
  public void SetViewportSize(Viewport viewport)
  {
    gl.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);
  }

  public void SetBlendState(BlendState state)
  {
    throw new NotImplementedException();
  }

  public void ClearColorBuffer(Color color)
  {
    gl.ClearColor(color.R, color.G, color.B, color.A);
    gl.Clear(ClearBufferMask.ColorBufferBit);
  }

  public void ClearDepthBuffer()
  {
    gl.ClearDepth(1.0f);
    gl.Clear(ClearBufferMask.DepthBufferBit);
  }

  public void FlushToDevice()
  {
    gl.Flush();
  }

  public GraphicsHandle CreateBuffer(BufferType type)
  {
    return new GraphicsHandle(gl.GenBuffer());
  }

  public unsafe Memory<T> ReadBufferData<T>(GraphicsHandle handle, BufferType type, nint offset, int length)
    where T : unmanaged
  {
    var kind = ConvertBufferType(type);
    var result = new T[length];

    gl.BindBuffer(kind, handle);

    fixed (T* pointer = result)
    {
      gl.GetBufferSubData(kind, offset * sizeof(T), (uint)(length * sizeof(T)), pointer);
    }

    return result;
  }


  public unsafe void WriteBufferData<T>(GraphicsHandle handle, BufferType type, ReadOnlySpan<T> data, BufferUsage usage)
    where T : unmanaged
  {
    var kind = ConvertBufferType(type);
    var bytes = (uint)(data.Length * sizeof(T));

    var bufferUsage = usage switch
    {
      BufferUsage.Static => BufferUsageARB.StaticDraw,
      BufferUsage.Dynamic => BufferUsageARB.DynamicDraw,

      _ => throw new ArgumentOutOfRangeException(nameof(usage), usage, null)
    };

    gl.BindBuffer(kind, handle);

    fixed (T* pointer = data)
    {
      gl.BufferData(kind, bytes, pointer, bufferUsage);
    }
  }

  public unsafe void WriteBufferSubData<T>(GraphicsHandle handle, BufferType type, IntPtr offset, ReadOnlySpan<T> data)
    where T : unmanaged
  {
    var kind = ConvertBufferType(type);
    var bytes = (uint)(data.Length * sizeof(T));

    gl.BindBuffer(kind, handle);

    fixed (T* pointer = data)
    {
      gl.BufferSubData(kind, offset, bytes, pointer);
    }
  }

  public void DeleteBuffer(GraphicsHandle handle)
  {
    gl.DeleteBuffer(handle);
  }

  public GraphicsHandle CreateTexture(TextureFilterMode filterMode, TextureWrapMode wrapMode)
  {
    throw new NotImplementedException();
  }

  public Memory<T> ReadTextureData<T>(GraphicsHandle handle, int mipLevel = 0)
    where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void ReadTextureData<T>(GraphicsHandle handle, Span<T> buffer, int mipLevel = 0)
    where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public Memory<T> ReadTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, int width, int height, int mipLevel = 0)
    where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void ReadTextureSubData<T>(GraphicsHandle handle, Span<T> buffer, int offsetX, int offsetY, int width, int height, int mipLevel = 0)
    where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void WriteTextureData<T>(GraphicsHandle handle, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0)
    where T : unmanaged
  {
    throw new NotImplementedException();
  }

  public void WriteTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0)
    where T : unmanaged
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

  public void DrawMesh(GraphicsHandle mesh, int vertexCount, int indexCount, MeshType meshType, Type indexType)
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

  public ReadOnlySlice<ShaderAttributeMetadata> GetShaderAttributeMetadata(GraphicsHandle handle)
  {
    throw new NotImplementedException();
  }

  public ReadOnlySlice<ShaderUniformMetadata> GetShaderUniformMetadata(GraphicsHandle handle)
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

  public void SetShaderSampler(GraphicsHandle handle, int location, GraphicsHandle texture, int samplerSlot)
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

  public GraphicsHandle CreateFrameBuffer(GraphicsHandle colorAttachment)
  {
    throw new NotImplementedException();
  }

  public void SetActiveFrameBuffer(GraphicsHandle handle)
  {
    throw new NotImplementedException();
  }

  public void SetDefaultFrameBuffer()
  {
    throw new NotImplementedException();
  }

  public void DeleteFrameBuffer(GraphicsHandle handle)
  {
    throw new NotImplementedException();
  }

  public void Dispose()
  {
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static BufferTargetARB ConvertBufferType(BufferType type)
  {
    return type switch
    {
      BufferType.Vertex => BufferTargetARB.ArrayBuffer,
      BufferType.Index => BufferTargetARB.ElementArrayBuffer,

      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
  }
}
