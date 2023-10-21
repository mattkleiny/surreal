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

  public void SetBlendState(BlendState? state)
  {
    if (state != null)
    {
      var sFactor = ConvertBlendFactor(state.Value.Source);
      var dFactor = ConvertBlendFactor(state.Value.Target);

      gl.Enable(EnableCap.Blend);
      gl.BlendFunc(sFactor, dFactor);
    }
    else
    {
      gl.Disable(EnableCap.Blend);
    }
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
      gl.GetBufferSubData(
        target: kind,
        offset: offset * sizeof(T),
        size: (uint)(length * sizeof(T)),
        data: pointer
      );
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
    var texture = gl.GenTexture();

    gl.BindTexture(TextureTarget.Texture2D, texture);

    var textureFilterMode = ConvertTextureFilterMode(filterMode);
    var textureWrapMode = ConvertTextureWrapMode(wrapMode);

    gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.GenerateMipmapSgis, 0);
    gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, textureFilterMode);
    gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, textureFilterMode);
    gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, textureWrapMode);
    gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, textureWrapMode);

    return new GraphicsHandle(texture);
  }

  public unsafe Memory<T> ReadTextureData<T>(GraphicsHandle handle, int mipLevel = 0)
    where T : unmanaged
  {
    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));

    gl.BindTexture(TextureTarget.Texture2D, handle);
    gl.GetTexParameterI(TextureTarget.Texture2D, GetTextureParameter.TextureWidth, out int width);
    gl.GetTexParameterI(TextureTarget.Texture2D, GetTextureParameter.TextureHeight, out int height);

    var results = new T[width * height];

    fixed (T* pointer = results)
    {
      gl.GetTexImage(
        target: TextureTarget.Texture2D,
        level: mipLevel,
        format: pixelFormat,
        type: pixelType,
        pixels: pointer
      );
    }

    return results;
  }

  public unsafe void ReadTextureData<T>(GraphicsHandle handle, Span<T> buffer, int mipLevel = 0)
    where T : unmanaged
  {
    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));

    gl.BindTexture(TextureTarget.Texture2D, handle);

    fixed (T* pointer = buffer)
    {
      gl.GetTexImage(
        target: TextureTarget.Texture2D,
        level: mipLevel,
        format: pixelFormat,
        type: pixelType,
        pixels: pointer
      );
    }
  }

  public unsafe Memory<T> ReadTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, int mipLevel = 0)
    where T : unmanaged
  {
    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));

    gl.BindTexture(TextureTarget.Texture2D, handle);

    var results = new T[width * height];

    fixed (T* pointer = results)
    {
      gl.GetTextureSubImage(
        texture: handle,
        level: mipLevel,
        xoffset: offsetX,
        yoffset: offsetY,
        zoffset: 0,
        width: width,
        height: height,
        depth: 1,
        format: pixelFormat,
        type: pixelType,
        pixels: pointer,
        bufSize: (uint)results.Length
      );
    }

    return results;
  }

  public unsafe void ReadTextureSubData<T>(GraphicsHandle handle, Span<T> buffer, int offsetX, int offsetY, uint width, uint height, int mipLevel = 0)
    where T : unmanaged
  {
    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));

    gl.BindTexture(TextureTarget.Texture2D, handle);

    fixed (T* pointer = buffer)
    {
      gl.GetTextureSubImage(
        texture: handle,
        level: mipLevel,
        xoffset: offsetX,
        yoffset: offsetY,
        zoffset: 0,
        width: width,
        height: height,
        depth: 1,
        format: pixelFormat,
        type: pixelType,
        pixels: pointer,
        bufSize: (uint)buffer.Length
      );
    }
  }

  public unsafe void WriteTextureData<T>(GraphicsHandle handle, uint width, uint height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0)
    where T : unmanaged
  {
    var internalFormat = GetInternalFormat(format);
    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));

    gl.BindTexture(TextureTarget.Texture2D, handle);

    fixed (T* pointer = pixels)
    {
      gl.TexImage2D(
        target: TextureTarget.Texture2D,
        level: mipLevel,
        internalformat: internalFormat,
        width: width,
        height: height,
        border: 0,
        format: pixelFormat,
        type: pixelType,
        pixels: pointer
      );
    }
  }

  public unsafe void WriteTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, ReadOnlySpan<T> pixels, TextureFormat format,
    int mipLevel = 0)
    where T : unmanaged
  {
    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));

    gl.BindTexture(TextureTarget.Texture2D, handle);

    fixed (T* pointer = pixels)
    {
      gl.TexSubImage2D(
        TextureTarget.Texture2D,
        mipLevel,
        width: width,
        height: height,
        xoffset: offsetX,
        yoffset: offsetY,
        format: pixelFormat,
        type: pixelType,
        pixels: pointer
      );
    }
  }

  public void SetTextureFilterMode(GraphicsHandle handle, TextureFilterMode mode)
  {
    gl.BindTexture(TextureTarget.Texture2D, handle);

    var textureFilterMode = ConvertTextureFilterMode(mode);

    gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, textureFilterMode);
    gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, textureFilterMode);
  }

  public void SetTextureWrapMode(GraphicsHandle handle, TextureWrapMode mode)
  {
    gl.BindTexture(TextureTarget.Texture2D, handle);

    var textureWrapMode = ConvertTextureWrapMode(mode);

    gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, textureWrapMode);
    gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, textureWrapMode);
  }

  public void DeleteTexture(GraphicsHandle handle)
  {
    gl.DeleteTexture(handle);
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
        index: (uint)index,
        size: attribute.Count,
        type: ConvertVertexType(attribute.Type),
        normalized: attribute.ShouldNormalize,
        stride: descriptors.Stride,
        pointer: (void*)attribute.Offset
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
  private static BlendingFactor ConvertBlendFactor(BlendMode mode)
  {
    return mode switch
    {
      BlendMode.SourceColor => BlendingFactor.SrcColor,
      BlendMode.TargetColor => BlendingFactor.DstColor,
      BlendMode.SourceAlpha => BlendingFactor.SrcAlpha,
      BlendMode.TargetAlpha => BlendingFactor.DstAlpha,
      BlendMode.OneMinusSourceColor => BlendingFactor.OneMinusSrcAlpha,
      BlendMode.OneMinusTargetColor => BlendingFactor.OneMinusDstColor,
      BlendMode.OneMinusSourceAlpha => BlendingFactor.OneMinusSrcAlpha,
      BlendMode.OneMinusTargetAlpha => BlendingFactor.OneMinusDstAlpha,

      _ => throw new InvalidOperationException($"An unexpected blend mode was specified {mode}")
    };
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
  private static int ConvertTextureFilterMode(TextureFilterMode filterMode)
  {
    return filterMode switch
    {
      TextureFilterMode.Point => (int)GLEnum.Nearest,
      TextureFilterMode.Linear => (int)GLEnum.Linear,

      _ => throw new ArgumentOutOfRangeException(nameof(filterMode), filterMode, null)
    };
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int ConvertTextureWrapMode(TextureWrapMode wrapMode)
  {
    return wrapMode switch
    {
      TextureWrapMode.Clamp => (int)GLEnum.ClampToEdge,
      TextureWrapMode.Repeat => (int)GLEnum.MirroredRepeat,

      _ => throw new ArgumentOutOfRangeException(nameof(wrapMode), wrapMode, null)
    };
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int GetInternalFormat(TextureFormat format)
  {
    return format switch
    {
      // integral
      TextureFormat.R8 => (int)GLEnum.R8,
      TextureFormat.Rgb8 => (int)GLEnum.Rgb8,
      TextureFormat.Rgba8 => (int)GLEnum.Rgba8,

      // floating
      TextureFormat.R => (int)GLEnum.Red,
      TextureFormat.Rgb => (int)GLEnum.Rgb,
      TextureFormat.Rgba => (int)GLEnum.Rgba,

      _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
    };
  }

  private static (PixelFormat Format, PixelType Type) GetPixelFormatAndType(Type type)
  {
    // integral
    if (type == typeof(byte)) return (PixelFormat.Red, PixelType.UnsignedByte);
    if (type == typeof(Point3)) return (PixelFormat.Rgb, PixelType.UnsignedByte);
    if (type == typeof(Color32)) return (PixelFormat.Rgba, PixelType.UnsignedByte);

    // floating
    if (type == typeof(float)) return (PixelFormat.Red, PixelType.Float);
    if (type == typeof(Vector3)) return (PixelFormat.Rgb, PixelType.Float);
    if (type == typeof(Vector4)) return (PixelFormat.Rgba, PixelType.Float);
    if (type == typeof(Color)) return (PixelFormat.Rgba, PixelType.Float);

    throw new InvalidOperationException($"An unrecognized pixel type was provided: {type}");
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

  private static DrawElementsType ConvertElementType(Type type)
  {
    if (type == typeof(byte)) return DrawElementsType.UnsignedByte;
    if (type == typeof(uint)) return DrawElementsType.UnsignedInt;
    if (type == typeof(ushort)) return DrawElementsType.UnsignedShort;

    throw new InvalidOperationException($"An unrecognized index type was provided: {type}");
  }
}
