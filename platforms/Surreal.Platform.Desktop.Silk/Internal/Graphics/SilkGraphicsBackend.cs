using Silk.NET.OpenGL;
using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Maths;
using TextureWrapMode = Surreal.Graphics.Textures.TextureWrapMode;

namespace Surreal.Graphics;

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

  public unsafe GraphicsHandle CreateMesh(GraphicsHandle vertices, GraphicsHandle indices, VertexDescriptorSet descriptors)
  {
    var array = gl.GenVertexArray();

    gl.BindVertexArray(array);

    gl.BindBuffer(BufferTargetARB.ArrayBuffer, vertices);
    gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, indices);

    // N.B: assumes ordered in the order they appear in location binding
    for (var index = 0; index < descriptors.Length; index++)
    {
      var attribute = descriptors[index];

      gl.VertexAttribPointer(
        (uint)index,
        attribute.Count,
        ConvertVertexType(attribute.Type),
        attribute.ShouldNormalize,
        descriptors.Stride,
        (void*)attribute.Offset
      );
      gl.EnableVertexAttribArray((uint)index);
    }

    gl.BindVertexArray(0);

    return new GraphicsHandle(array);
  }

  public void DrawMesh(GraphicsHandle mesh, uint vertexCount, uint indexCount, MeshType meshType, Type indexType)
  {
    gl.BindVertexArray(mesh);

    var primitiveType = ConvertMeshType(meshType);

    if (indexCount > 0)
    {
      var elementType = ConvertElementType(indexType);

      gl.DrawElements(primitiveType, indexCount, elementType, 0);
    }
    else
    {
      gl.DrawArrays(primitiveType, 0, vertexCount);
    }

    gl.BindVertexArray(0);
  }

  public void DeleteMesh(GraphicsHandle handle)
  {
    gl.DeleteVertexArray(handle);
  }

  public GraphicsHandle CreateShader()
  {
    return new GraphicsHandle(gl.CreateProgram());
  }

  public int GetShaderUniformLocation(GraphicsHandle handle, string name)
  {
    return gl.GetUniformLocation(handle, name);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, int value)
  {
    gl.ProgramUniform1(handle, location, value);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, float value)
  {
    gl.ProgramUniform1(handle, location, value);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Point2 value)
  {
    gl.ProgramUniform2(handle, location, value.X, value.Y);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Point3 value)
  {
    gl.ProgramUniform3(handle, location, value.X, value.Y, value.Z);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Point4 value)
  {
    gl.ProgramUniform4(handle, location, value.X, value.Y, value.Z, value.W);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Vector2 value)
  {
    gl.ProgramUniform2(handle, location, value.X, value.Y);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Vector3 value)
  {
    gl.ProgramUniform3(handle, location, value.X, value.Y, value.Z);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Vector4 value)
  {
    gl.ProgramUniform4(handle, location, value.X, value.Y, value.Z, value.W);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Quaternion value)
  {
    gl.ProgramUniform4(handle, location, value.X, value.Y, value.Z, value.W);
  }

  public unsafe void SetShaderUniform(GraphicsHandle handle, int location, in Matrix3x2 value)
  {
    fixed (float* pointer = &value.M11)
    {
      gl.ProgramUniformMatrix3x2(handle, location, 1, transpose: true, pointer);
    }
  }

  public unsafe void SetShaderUniform(GraphicsHandle handle, int location, in Matrix4x4 value)
  {
    fixed (float* pointer = &value.M11)
    {
      gl.ProgramUniformMatrix4(handle, location, 1, transpose: true, pointer);
    }
  }

  public void SetShaderSampler(GraphicsHandle handle, int location, GraphicsHandle texture, int samplerSlot)
  {
    gl.ActiveTexture(TextureUnit.Texture0 + samplerSlot);
    gl.BindTexture(TextureTarget.Texture2D, texture);
    gl.ProgramUniform1(handle, location, samplerSlot);
  }

  public void SetActiveShader(GraphicsHandle handle)
  {
    gl.UseProgram(handle);
  }

  public void DeleteShader(GraphicsHandle handle)
  {
    gl.DeleteProgram(handle);
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

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static VertexAttribPointerType ConvertVertexType(VertexType attributeType)
  {
    return attributeType switch
    {
      VertexType.UnsignedByte => VertexAttribPointerType.UnsignedByte,
      VertexType.Short => VertexAttribPointerType.Short,
      VertexType.UnsignedShort => VertexAttribPointerType.UnsignedShort,
      VertexType.Int => VertexAttribPointerType.Int,
      VertexType.UnsignedInt => VertexAttribPointerType.UnsignedInt,
      VertexType.Float => VertexAttribPointerType.Float,
      VertexType.Double => VertexAttribPointerType.Double,

      _ => throw new ArgumentOutOfRangeException(nameof(attributeType), attributeType, null)
    };
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static PrimitiveType ConvertMeshType(MeshType meshType)
  {
    return meshType switch
    {
      MeshType.Points => PrimitiveType.Points,
      MeshType.Lines => PrimitiveType.Lines,
      MeshType.LineStrip => PrimitiveType.LineStrip,
      MeshType.LineLoop => PrimitiveType.LineLoop,
      MeshType.Triangles => PrimitiveType.Triangles,

      _ => throw new ArgumentOutOfRangeException(nameof(meshType), meshType, null)
    };
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static DrawElementsType ConvertElementType(Type type)
  {
    if (type == typeof(byte)) return DrawElementsType.UnsignedByte;
    if (type == typeof(uint)) return DrawElementsType.UnsignedInt;
    if (type == typeof(ushort)) return DrawElementsType.UnsignedShort;

    throw new InvalidOperationException($"An unrecognized index type was provided: {type}");
  }
}
