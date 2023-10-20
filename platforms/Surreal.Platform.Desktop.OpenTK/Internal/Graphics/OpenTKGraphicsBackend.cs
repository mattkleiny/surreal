using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Surreal.Collections;
using Surreal.Colors;
using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Matrix3x2 = System.Numerics.Matrix3x2;
using Quaternion = System.Numerics.Quaternion;
using TextureWrapMode = Surreal.Graphics.Textures.TextureWrapMode;
using UniformType = Surreal.Graphics.Shaders.UniformType;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace Surreal.Internal.Graphics;

/// <summary>
/// The <see cref="IGraphicsContext" /> for the OpenTK backend (OpenGL).
/// </summary>
internal sealed class OpenTKGraphicsBackend : IGraphicsBackend
{
  public void SetViewportSize(Viewport viewport)
  {
    GL.Viewport(viewport.X, viewport.Y, (int)viewport.Width, (int)viewport.Height);
  }

  public void SetBlendState(BlendState state)
  {
    static BlendingFactor ConvertBlendFactor(BlendMode mode)
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

    if (state.IsEnabled)
    {
      var sfactor = ConvertBlendFactor(state.Source);
      var dfactor = ConvertBlendFactor(state.Target);

      GL.Enable(EnableCap.Blend);
      GL.BlendFunc(sfactor, dfactor);
    }
    else
    {
      GL.Disable(EnableCap.Blend);
    }
  }

  public void ClearColorBuffer(Color color)
  {
    GL.ClearColor(color.R, color.G, color.B, color.A);
    GL.Clear(ClearBufferMask.ColorBufferBit);
  }

  public void ClearDepthBuffer()
  {
    GL.Clear(ClearBufferMask.DepthBufferBit);
  }

  public void FlushToDevice()
  {
    GL.Flush();
  }

  public GraphicsHandle CreateBuffer(BufferType type)
  {
    var buffer = GL.GenBuffer();

    return new GraphicsHandle(buffer.Handle);
  }

  public void DeleteBuffer(GraphicsHandle handle)
  {
    var buffer = new BufferHandle(handle);

    GL.DeleteBuffer(buffer);
  }

  public unsafe Memory<T> ReadBufferData<T>(GraphicsHandle handle, BufferType type, nint offset, int length)
    where T : unmanaged
  {
    var buffer = new BufferHandle(handle);
    var kind = ConvertBufferType(type);
    var result = new T[length];

    GL.BindBuffer(kind, buffer);

    fixed (T* pointer = result)
    {
      GL.GetBufferSubData(kind, offset * sizeof(T), length * sizeof(T), pointer);
    }

    return result;
  }

  public unsafe void WriteBufferData<T>(GraphicsHandle handle, BufferType type, ReadOnlySpan<T> data, BufferUsage usage)
    where T : unmanaged
  {
    var buffer = new BufferHandle(handle);
    var kind = ConvertBufferType(type);
    var bytes = data.Length * sizeof(T);

    var bufferUsage = usage switch
    {
      BufferUsage.Static => BufferUsageARB.StaticDraw,
      BufferUsage.Dynamic => BufferUsageARB.DynamicDraw,

      _ => throw new ArgumentOutOfRangeException(nameof(usage), usage, null)
    };

    GL.BindBuffer(kind, buffer);

    fixed (T* pointer = data)
    {
      GL.BufferData(kind, bytes, pointer, bufferUsage);
    }
  }

  public unsafe void WriteBufferSubData<T>(GraphicsHandle handle, BufferType type, nint offset, ReadOnlySpan<T> data) where T : unmanaged
  {
    var buffer = new BufferHandle(handle);
    var kind = ConvertBufferType(type);
    var bytes = data.Length * sizeof(T);

    GL.BindBuffer(kind, buffer);

    fixed (T* pointer = data)
    {
      GL.BufferSubData(kind, offset, bytes, pointer);
    }
  }

  public GraphicsHandle CreateTexture(TextureFilterMode filterMode, TextureWrapMode wrapMode)
  {
    var texture = GL.GenTexture();

    GL.BindTexture(TextureTarget.Texture2d, texture);

    var textureFilterMode = ConvertTextureFilterMode(filterMode);
    var textureWrapMode = ConvertTextureWrapMode(wrapMode);

    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.GenerateMipmap, 0);
    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, textureFilterMode);
    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, textureFilterMode);
    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, textureWrapMode);
    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, textureWrapMode);

    return new GraphicsHandle(texture.Handle);
  }

  public unsafe Memory<T> ReadTextureData<T>(GraphicsHandle handle, int mipLevel = 0)
    where T : unmanaged
  {
    var texture = new TextureHandle(handle);
    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));

    var width = 0;
    var height = 0;

    GL.BindTexture(TextureTarget.Texture2d, texture);
    GL.GetTexParameteri(TextureTarget.Texture2d, GetTextureParameter.TextureWidth, ref width);
    GL.GetTexParameteri(TextureTarget.Texture2d, GetTextureParameter.TextureHeight, ref height);

    var results = new T[width * height];

    fixed (T* pointer = results)
    {
      GL.GetTexImage(
        TextureTarget.Texture2d,
        mipLevel,
        pixelFormat,
        pixelType,
        pointer
      );
    }

    return results;
  }

  public unsafe void ReadTextureData<T>(GraphicsHandle handle, Span<T> buffer, int mipLevel = 0)
    where T : unmanaged
  {
    var texture = new TextureHandle(handle);
    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));

    GL.BindTexture(TextureTarget.Texture2d, texture);

    fixed (T* pointer = buffer)
    {
      GL.GetTexImage(
        TextureTarget.Texture2d,
        mipLevel,
        pixelFormat,
        pixelType,
        pointer
      );
    }
  }

  public unsafe Memory<T> ReadTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, int width, int height, int mipLevel = 0)
    where T : unmanaged
  {
    var texture = new TextureHandle(handle);
    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));

    GL.BindTexture(TextureTarget.Texture2d, texture);

    var results = new T[width * height];

    fixed (T* pointer = results)
    {
      GL.GetTextureSubImage(
        texture,
        mipLevel,
        offsetX,
        offsetY,
        0,
        width,
        height,
        1,
        pixelFormat,
        pixelType,
        pixels: pointer,
        bufSize: results.Length
      );
    }

    return results;
  }

  public unsafe void ReadTextureSubData<T>(GraphicsHandle handle, Span<T> buffer, int offsetX, int offsetY, int width, int height, int mipLevel = 0)
    where T : unmanaged
  {
    var texture = new TextureHandle(handle);
    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));

    GL.BindTexture(TextureTarget.Texture2d, texture);

    fixed (T* pointer = buffer)
    {
      GL.GetTextureSubImage(
        texture,
        mipLevel,
        offsetX,
        offsetY,
        0,
        width,
        height,
        1,
        pixelFormat,
        pixelType,
        pixels: pointer,
        bufSize: buffer.Length
      );
    }
  }

  public unsafe void WriteTextureData<T>(GraphicsHandle handle, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel)
    where T : unmanaged
  {
    var texture = new TextureHandle(handle);

    var internalFormat = GetInternalFormat(format);
    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));

    GL.BindTexture(TextureTarget.Texture2d, texture);

    fixed (T* pointer = pixels)
    {
      GL.TexImage2D(
        TextureTarget.Texture2d,
        mipLevel,
        internalFormat,
        width,
        height,
        0,
        pixelFormat,
        pixelType,
        pointer
      );
    }
  }

  public unsafe void WriteTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0)
    where T : unmanaged
  {
    var texture = new TextureHandle(handle);

    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));

    GL.BindTexture(TextureTarget.Texture2d, texture);

    fixed (T* pointer = pixels)
    {
      GL.TexSubImage2D(
        TextureTarget.Texture2d,
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
    var texture = new TextureHandle(handle);

    GL.BindTexture(TextureTarget.Texture2d, texture);

    var textureFilterMode = ConvertTextureFilterMode(mode);

    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, textureFilterMode);
    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, textureFilterMode);
  }

  public void SetTextureWrapMode(GraphicsHandle handle, TextureWrapMode mode)
  {
    var texture = new TextureHandle(handle);

    GL.BindTexture(TextureTarget.Texture2d, texture);

    var textureWrapMode = ConvertTextureWrapMode(mode);

    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, textureWrapMode);
    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, textureWrapMode);
  }

  public void DeleteTexture(GraphicsHandle handle)
  {
    var texture = new TextureHandle(handle);

    GL.DeleteTexture(texture);
  }

  public GraphicsHandle CreateMesh(GraphicsHandle vertices, GraphicsHandle indices, VertexDescriptorSet descriptors)
  {
    var array = GL.GenVertexArray();

    GL.BindVertexArray(array);

    GL.BindBuffer(BufferTargetARB.ArrayBuffer, new BufferHandle(vertices));
    GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, new BufferHandle(indices));

    // N.B: assumes ordered in the order they appear in location binding
    for (var index = 0; index < descriptors.Length; index++)
    {
      var attribute = descriptors[index];

      GL.VertexAttribPointer(
        (uint)index,
        attribute.Count,
        ConvertVertexType(attribute.Type),
        attribute.ShouldNormalize,
        descriptors.Stride,
        attribute.Offset
      );
      GL.EnableVertexAttribArray((uint)index);
    }

    GL.BindVertexArray(VertexArrayHandle.Zero);

    return new GraphicsHandle(array.Handle);
  }

  public void DrawMesh(GraphicsHandle mesh, int vertexCount, int indexCount, MeshType meshType, Type indexType)
  {
    var array = new VertexArrayHandle(mesh);

    GL.BindVertexArray(array);

    var primitiveType = ConvertMeshType(meshType);

    if (indexCount > 0)
    {
      var elementType = ConvertElementType(indexType);

      GL.DrawElements(primitiveType, indexCount, elementType, 0);
    }
    else
    {
      GL.DrawArrays(primitiveType, 0, vertexCount);
    }

    GL.BindVertexArray(VertexArrayHandle.Zero);
  }

  public void DeleteMesh(GraphicsHandle handle)
  {
    var array = new VertexArrayHandle(handle);

    GL.DeleteVertexArray(array);
  }

  public GraphicsHandle CreateShader()
  {
    var shader = GL.CreateProgram();

    return new GraphicsHandle(shader.Handle);
  }

  public unsafe ReadOnlySlice<ShaderAttributeMetadata> GetShaderAttributeMetadata(GraphicsHandle handle)
  {
    var program = new ProgramHandle(handle);
    var count = 0;

    GL.GetProgramiv(program, ProgramPropertyARB.ActiveUniforms, &count);

    var results = new ShaderAttributeMetadata[count];

    for (var index = 0; index < count; index++)
    {
      var length = 0;
      var size = 0;
      var type = default(AttributeType);

      GL.GetActiveAttrib(
        program,
        (uint)index,
        int.MaxValue,
        ref length,
        ref size,
        ref type,
        out var name
      );
      if (string.IsNullOrEmpty(name))
      {
        continue;
      }

      results[index] = new ShaderAttributeMetadata(name, index, length, size, type switch
      {
        AttributeType.Int => UniformType.Integer,
        AttributeType.Float => UniformType.Float,
        AttributeType.IntVec2 => UniformType.Point2,
        AttributeType.IntVec3 => UniformType.Point3,
        AttributeType.FloatVec2 => UniformType.Vector2,
        AttributeType.FloatVec3 => UniformType.Vector3,
        AttributeType.FloatVec4 => UniformType.Vector4,
        AttributeType.FloatMat3x2 => UniformType.Matrix3X2,
        AttributeType.FloatMat4 => UniformType.Matrix4X4,
        AttributeType.Sampler2d => UniformType.Texture,

        _ => throw new InvalidOperationException($"An unexpected type was encountered: {type}")
      });
    }

    return results;
  }

  public unsafe ReadOnlySlice<ShaderUniformMetadata> GetShaderUniformMetadata(GraphicsHandle handle)
  {
    var program = new ProgramHandle(handle);
    var count = 0;

    GL.GetProgramiv(program, ProgramPropertyARB.ActiveUniforms, &count);

    var results = new ShaderUniformMetadata[count];

    for (var index = 0; index < count; index++)
    {
      var length = 0;
      var size = 0;
      var type = default(OpenTK.Graphics.OpenGL.UniformType);

      GL.GetActiveUniform(
        program,
        (uint)index,
        int.MaxValue,
        ref length,
        ref size,
        ref type,
        out var name
      );
      if (string.IsNullOrEmpty(name))
      {
        continue;
      }

      var location = GL.GetUniformLocation(program, name);

      results[index] = new ShaderUniformMetadata(name, location, length, size, type switch
      {
        OpenTK.Graphics.OpenGL.UniformType.Int => UniformType.Integer,
        OpenTK.Graphics.OpenGL.UniformType.Float => UniformType.Float,
        OpenTK.Graphics.OpenGL.UniformType.IntVec2 => UniformType.Point2,
        OpenTK.Graphics.OpenGL.UniformType.IntVec3 => UniformType.Point3,
        OpenTK.Graphics.OpenGL.UniformType.FloatVec2 => UniformType.Vector2,
        OpenTK.Graphics.OpenGL.UniformType.FloatVec3 => UniformType.Vector3,
        OpenTK.Graphics.OpenGL.UniformType.FloatVec4 => UniformType.Vector4,
        OpenTK.Graphics.OpenGL.UniformType.FloatMat3x2 => UniformType.Matrix3X2,
        OpenTK.Graphics.OpenGL.UniformType.FloatMat4 => UniformType.Matrix4X4,
        OpenTK.Graphics.OpenGL.UniformType.Sampler2d => UniformType.Texture,

        _ => throw new InvalidOperationException($"An unexpected type was encountered: {type}")
      });
    }

    return results;
  }

  public int GetShaderUniformLocation(GraphicsHandle handle, string name)
  {
    return GL.GetUniformLocation(new ProgramHandle(handle), name);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, int value)
  {
    GL.ProgramUniform1i(new ProgramHandle(handle), location, value);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, float value)
  {
    GL.ProgramUniform1f(new ProgramHandle(handle), location, value);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Point2 value)
  {
    GL.ProgramUniform2i(new ProgramHandle(handle), location, value.X, value.Y);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Point3 value)
  {
    GL.ProgramUniform3i(new ProgramHandle(handle), location, value.X, value.Y, value.Z);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Point4 value)
  {
    GL.ProgramUniform4i(new ProgramHandle(handle), location, value.X, value.Y, value.Z, value.W);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Vector2 value)
  {
    GL.ProgramUniform2f(new ProgramHandle(handle), location, value.X, value.Y);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Vector3 value)
  {
    GL.ProgramUniform3f(new ProgramHandle(handle), location, value.X, value.Y, value.Z);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Vector4 value)
  {
    GL.ProgramUniform4f(new ProgramHandle(handle), location, value.X, value.Y, value.Z, value.W);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Quaternion value)
  {
    GL.ProgramUniform4f(new ProgramHandle(handle), location, value.X, value.Y, value.Z, value.W);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, in Matrix3x2 value)
  {
    var result = Unsafe.As<Matrix3x2, OpenTK.Mathematics.Matrix3x2>(ref Unsafe.AsRef(in value));

    GL.ProgramUniformMatrix3x2f(new ProgramHandle(handle), location, 1, true, stackalloc[] { result });
  }

  public unsafe void SetShaderUniform(GraphicsHandle handle, int location, in Matrix4x4 value)
  {
    var result = Unsafe.As<Matrix4x4, Matrix4>(ref Unsafe.AsRef(in value));

    GL.ProgramUniformMatrix4f(new ProgramHandle(handle), location, 1, true, stackalloc[] { result });
  }

  public void SetShaderSampler(GraphicsHandle handle, int location, GraphicsHandle texture, int samplerSlot)
  {
    GL.ActiveTexture(TextureUnit.Texture0 + (uint)samplerSlot);
    GL.BindTexture(TextureTarget.Texture2d, new TextureHandle(texture));
    GL.ProgramUniform1i(new ProgramHandle(handle), location, samplerSlot);
  }

  public void SetActiveShader(GraphicsHandle handle)
  {
    GL.UseProgram(new ProgramHandle(handle));
  }

  public void DeleteShader(GraphicsHandle handle)
  {
    var program = new ProgramHandle(handle);

    GL.DeleteProgram(program);
  }

  public GraphicsHandle CreateFrameBuffer(GraphicsHandle colorAttachment)
  {
    var framebuffer = GL.GenFramebuffer();

    GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, new TextureHandle(colorAttachment), 0);

    if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
    {
      throw new GraphicsException("The frame buffer is incomplete; are the texture formats compatible?");
    }

    GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandle.Zero);

    return new GraphicsHandle(framebuffer.Handle);
  }

  public void SetActiveFrameBuffer(GraphicsHandle handle)
  {
    GL.BindFramebuffer(FramebufferTarget.Framebuffer, new FramebufferHandle(handle));
  }

  public void SetDefaultFrameBuffer()
  {
    GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandle.Zero);
  }

  public void DeleteFrameBuffer(GraphicsHandle handle)
  {
    var framebuffer = new FramebufferHandle(handle);

    GL.DeleteFramebuffer(framebuffer);
  }

  public void LinkShader(GraphicsHandle handle, OpenTKShaderSet shaderSet)
  {
    var program = new ProgramHandle(handle);
    var shaderIds = new ShaderHandle[shaderSet.Shaders.Length];

    GL.UseProgram(program);

    for (var i = 0; i < shaderSet.Shaders.Length; i++)
    {
      var (stage, code) = shaderSet.Shaders[i];
      var shader = shaderIds[i] = GL.CreateShader(stage);

      GL.ShaderSource(shader, code);
      GL.CompileShader(shader);

      var compileStatus = 0;

      GL.GetShaderi(shader, ShaderParameterName.CompileStatus, ref compileStatus);

      if (compileStatus != 1)
      {
        GL.GetShaderInfoLog(shader, out var errorLog);
        GL.DeleteShader(shader); // don't leak the shader

        throw new PlatformException($"An error occurred whilst compiling a {stage} shader from {shaderSet.Path}: {errorLog}");
      }

      GL.AttachShader(program, shader);
    }

    var linkStatus = 0;

    GL.LinkProgram(program);
    GL.GetProgrami(program, ProgramPropertyARB.LinkStatus, ref linkStatus);

    if (linkStatus != 1)
    {
      GL.GetProgramInfoLog(program, out var errorLog);
      GL.DeleteProgram(program); // don't leak the program

      throw new PlatformException($"An error occurred whilst linking a shader program from {shaderSet.Path}: {errorLog}");
    }

    // we're finished with the shaders, now
    foreach (var shaderId in shaderIds) GL.DeleteShader(shaderId);
  }

  private static int GetInternalFormat(TextureFormat format)
  {
    return format switch
    {
      // integral
      TextureFormat.R8 => (int)All.R8,
      TextureFormat.Rg8 => (int)All.Rg8,
      TextureFormat.Rgb8 => (int)All.Rgb8,
      TextureFormat.Rgba8 => (int)All.Rgba8,

      // floating
      TextureFormat.R => (int)All.R,
      TextureFormat.Rg => (int)All.Rg,
      TextureFormat.Rgb => (int)All.Rgb,
      TextureFormat.Rgba => (int)All.Rgba,

      _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
    };
  }

  private static (PixelFormat Format, PixelType Type) GetPixelFormatAndType(Type type)
  {
    // integral
    if (type == typeof(byte))
    {
      return (PixelFormat.Red, PixelType.UnsignedByte);
    }

    if (type == typeof(Point2))
    {
      return (PixelFormat.Rg, PixelType.UnsignedByte);
    }

    if (type == typeof(Point3))
    {
      return (PixelFormat.Rgb, PixelType.UnsignedByte);
    }

    if (type == typeof(Color32))
    {
      return (PixelFormat.Rgba, PixelType.UnsignedByte);
    }

    // floating
    if (type == typeof(float))
    {
      return (PixelFormat.Red, PixelType.Float);
    }

    if (type == typeof(Vector2))
    {
      return (PixelFormat.Rg, PixelType.Float);
    }

    if (type == typeof(Vector3))
    {
      return (PixelFormat.Rgb, PixelType.Float);
    }

    if (type == typeof(Vector4))
    {
      return (PixelFormat.Rgba, PixelType.Float);
    }

    if (type == typeof(Color))
    {
      return (PixelFormat.Rgba, PixelType.Float);
    }

    throw new InvalidOperationException($"An unrecognized pixel type was provided: {type}");
  }

  private static DrawElementsType ConvertElementType(Type type)
  {
    if (type == typeof(byte))
    {
      return DrawElementsType.UnsignedByte;
    }

    if (type == typeof(uint))
    {
      return DrawElementsType.UnsignedInt;
    }

    if (type == typeof(ushort))
    {
      return DrawElementsType.UnsignedShort;
    }

    throw new InvalidOperationException($"An unrecognized index type was provided: {type}");
  }

  private static BufferTargetARB ConvertBufferType(BufferType type)
  {
    return type switch
    {
      BufferType.Vertex => BufferTargetARB.ArrayBuffer,
      BufferType.Index => BufferTargetARB.ElementArrayBuffer,

      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
  }

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

  private static int ConvertTextureFilterMode(TextureFilterMode filterMode)
  {
    return filterMode switch
    {
      TextureFilterMode.Point => (int)All.Nearest,
      TextureFilterMode.Linear => (int)All.Linear,

      _ => throw new ArgumentOutOfRangeException(nameof(filterMode), filterMode, null)
    };
  }

  private static int ConvertTextureWrapMode(TextureWrapMode wrapMode)
  {
    return wrapMode switch
    {
      TextureWrapMode.Clamp => (int)All.ClampToEdge,
      TextureWrapMode.Repeat => (int)All.MirroredRepeat,

      _ => throw new ArgumentOutOfRangeException(nameof(wrapMode), wrapMode, null)
    };
  }

  public void Dispose()
  {
    // no-op
  }
}
