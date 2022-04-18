using System.Runtime.CompilerServices;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace Surreal.Internal.Graphics;

/// <summary>The <see cref="IGraphicsServer"/> for the OpenTK backend (OpenGL).</summary>
internal sealed class OpenTKGraphicsServer : IGraphicsServer, IHasNativeShaderSupport
{
  private readonly OpenTKShaderCompiler shaderCompiler = new();

  public void SetViewportSize(Viewport viewport)
  {
    GL.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);
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

  public GraphicsHandle CreateBuffer()
  {
    var buffer = GL.GenBuffer();

    return new GraphicsHandle(buffer.Handle);
  }

  public void DeleteBuffer(GraphicsHandle handle)
  {
    var buffer = new BufferHandle(handle);

    GL.DeleteBuffer(buffer);
  }

  public unsafe Memory<T> ReadBufferData<T>(GraphicsHandle handle, Range range)
    where T : unmanaged
  {
    var buffer = new BufferHandle(handle);

    int sizeInBytes = 0;

    GL.BindBuffer(BufferTargetARB.ArrayBuffer, buffer);
    GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref sizeInBytes);

    var stride = sizeof(T);
    var (offset, length) = range.GetOffsetAndLength(sizeInBytes / stride);

    var byteOffset = offset * stride;
    var result = new T[length];

    fixed (T* pointer = result)
    {
      GL.GetBufferSubData(BufferTargetARB.ArrayBuffer, new IntPtr(byteOffset), sizeInBytes, pointer);
    }

    return result;
  }

  public unsafe void WriteBufferData<T>(GraphicsHandle handle, ReadOnlySpan<T> data)
    where T : unmanaged
  {
    var buffer = new BufferHandle(handle);
    var bytes = data.Length * sizeof(T);

    fixed (T* pointer = data)
    {
      GL.BindBuffer(BufferTargetARB.ArrayBuffer, buffer);
      GL.BufferData(BufferTargetARB.ArrayBuffer, bytes, pointer, BufferUsageARB.StaticDraw);
    }
  }

  public GraphicsHandle CreateTexture()
  {
    var texture = GL.GenTexture();

    return new GraphicsHandle(texture.Handle);
  }

  public void DeleteTexture(GraphicsHandle handle)
  {
    var texture = new TextureHandle(handle);

    GL.DeleteTexture(texture);
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
        target: TextureTarget.Texture2d,
        level: mipLevel,
        format: pixelFormat,
        type: pixelType,
        pixels: pointer
      );
    }

    return results;
  }

  public unsafe void WriteTextureData<T>(GraphicsHandle handle, int width, int height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel)
    where T : unmanaged
  {
    var texture = new TextureHandle(handle);

    var (pixelFormat, pixelType) = GetPixelFormatAndType(typeof(T));
    var internalFormat = GetInternalFormat(format);

    GL.BindTexture(TextureTarget.Texture2d, texture);

    fixed (T* pointer = pixels)
    {
      GL.TexImage2D(
        target: TextureTarget.Texture2d,
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

  public void DrawMesh(GraphicsHandle shader, GraphicsHandle vertices, GraphicsHandle indices, VertexDescriptorSet descriptors, int vertexCount, int indexCount, MeshType meshType, Type indexType)
  {
    var primitiveType = ConvertMeshType(meshType);
    var elementType = ConvertElementType(indexType);

    var program = new ProgramHandle(shader);

    GL.UseProgram(program);

    BindVertexDescriptorSet(program, descriptors);

    GL.BindBuffer(BufferTargetARB.ArrayBuffer, new BufferHandle(vertices));
    GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, new BufferHandle(indices));

    if (indexCount > 0)
    {
      GL.DrawElements(primitiveType, indexCount, elementType, 0);
    }
    else
    {
      GL.DrawArrays(primitiveType, 0, vertexCount);
    }
  }

  public GraphicsHandle CreateShader()
  {
    var shader = GL.CreateProgram();

    return new GraphicsHandle(shader.Handle);
  }

  public void CompileShader(GraphicsHandle handle, ShaderDeclaration declaration)
  {
    var shaderSet = shaderCompiler.CompileShader(declaration);

    LinkShaderProgram(handle, shaderSet);
  }

  private static void LinkShaderProgram(GraphicsHandle handle, OpenTKShaderSet shaderSet)
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

    int linkStatus = 0;

    GL.LinkProgram(program);
    GL.GetProgrami(program, ProgramPropertyARB.LinkStatus, ref linkStatus);

    if (linkStatus != 1)
    {
      GL.GetProgramInfoLog(program, out var errorLog);
      GL.DeleteProgram(program); // don't leak the program

      throw new PlatformException($"An error occurred whilst linking a shader program from {shaderSet.Path}: {errorLog}");
    }

    // we're finished with the shaders, now
    foreach (var shaderId in shaderIds)
    {
      GL.DeleteShader(shaderId);
    }
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, int value)
  {
    var program = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);
    if (location == -1) return;

    GL.Uniform1i(location, value);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, float value)
  {
    var program = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);
    if (location == -1) return;

    GL.Uniform1f(location, value);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Point2 value)
  {
    var program = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);
    if (location == -1) return;

    GL.Uniform2i(location, value.X, value.Y);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Point3 value)
  {
    var program = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);
    if (location == -1) return;

    GL.Uniform3i(location, value.X, value.Y, value.Z);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector2 value)
  {
    var program = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);
    if (location == -1) return;

    GL.Uniform2f(location, value.X, value.Y);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector3 value)
  {
    var program = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);
    if (location == -1) return;

    GL.Uniform3f(location, value.X, value.Y, value.Z);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Vector4 value)
  {
    var program = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);
    if (location == -1) return;

    GL.Uniform4f(location, value.X, value.Y, value.Z, value.W);
  }

  public void SetShaderUniform(GraphicsHandle handle, string name, Quaternion value)
  {
    var program = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);
    if (location == -1) return;

    GL.Uniform4f(location, value.X, value.Y, value.Z, value.W);
  }

  public unsafe void SetShaderUniform(GraphicsHandle handle, string name, in Matrix3x2 value)
  {
    var program = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);
    if (location == -1) return;

    var pointer = (float*) Unsafe.AsPointer(ref Unsafe.AsRef(in value));

    GL.UniformMatrix4f(location, 1, false, new ReadOnlySpan<float>(pointer, 3 * 2));
  }

  public unsafe void SetShaderUniform(GraphicsHandle handle, string name, in Matrix4x4 value)
  {
    var program = new ProgramHandle(handle);
    var location = GL.GetAttribLocation(program, name);
    if (location == -1) return;

    var pointer = (float*) Unsafe.AsPointer(ref Unsafe.AsRef(in value));

    GL.UniformMatrix4f(location, 1, false, new ReadOnlySpan<float>(pointer, 4 * 4));
  }

  public void DeleteShader(GraphicsHandle handle)
  {
    var program = new ProgramHandle(handle);

    GL.DeleteProgram(program);
  }

  async ValueTask IHasNativeShaderSupport.CompileNativeShaderAsync(GraphicsHandle handle, VirtualPath path, CancellationToken cancellationToken)
  {
    var vertexPath = path.ChangeExtension("vert.glsl");
    var fragmentPath = path.ChangeExtension("frag.glsl");

    var vertexCode = await vertexPath.ReadAllTextAsync(Encoding.UTF8, cancellationToken);
    var fragmentCode = await fragmentPath.ReadAllTextAsync(Encoding.UTF8, cancellationToken);

    var shaderSet = new OpenTKShaderSet(path.ToString(), ImmutableArray.Create(
      new OpenTKShader(ShaderType.VertexShader, vertexCode),
      new OpenTKShader(ShaderType.FragmentShader, fragmentCode)
    ));

    LinkShaderProgram(handle, shaderSet);
  }

  private static void BindVertexDescriptorSet(ProgramHandle program, VertexDescriptorSet descriptors)
  {
    for (var i = 0; i < descriptors.Length; i++)
    {
      var attribute = descriptors[i];
      var location = GL.GetAttribLocation(program, attribute.Alias);

      if (location == -1)
      {
        continue; // no big deal
      }

      GL.VertexAttribPointer(
        index: (uint) location,
        size: attribute.Count,
        type: ConvertVertexType(attribute.Type),
        normalized: attribute.Normalized,
        stride: descriptors.Stride,
        offset: attribute.Offset
      );
      GL.EnableVertexAttribArray((uint) location);
    }
  }

  private static (PixelFormat Format, PixelType Type) GetPixelFormatAndType(Type type)
  {
    if (type == typeof(Color)) return (PixelFormat.Rgba, PixelType.Float);
    if (type == typeof(Color32)) return (PixelFormat.Rgba, PixelType.UnsignedByte);
    if (type == typeof(Vector3)) return (PixelFormat.Rgb, PixelType.Float);
    if (type == typeof(Vector4)) return (PixelFormat.Rgba, PixelType.Float);

    throw new InvalidOperationException($"An unrecognized pixel type was provided: {type}");
  }

  private static DrawElementsType ConvertElementType(Type type)
  {
    if (type == typeof(byte)) return DrawElementsType.UnsignedByte;
    if (type == typeof(uint)) return DrawElementsType.UnsignedInt;
    if (type == typeof(ushort)) return DrawElementsType.UnsignedShort;

    throw new InvalidOperationException($"An unrecognized index type was provided: {type}");
  }

  private static int GetInternalFormat(TextureFormat format)
  {
    return format switch
    {
      TextureFormat.Rgba8888 => (int) All.CompressedRgba,

      _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
    };
  }

  private static VertexAttribPointerType ConvertVertexType(VertexType attributeType)
  {
    return attributeType switch
    {
      VertexType.Byte          => VertexAttribPointerType.Byte,
      VertexType.UnsignedByte  => VertexAttribPointerType.UnsignedByte,
      VertexType.Short         => VertexAttribPointerType.Short,
      VertexType.UnsignedShort => VertexAttribPointerType.UnsignedShort,
      VertexType.Int           => VertexAttribPointerType.Int,
      VertexType.UnsignedInt   => VertexAttribPointerType.UnsignedInt,
      VertexType.Float         => VertexAttribPointerType.Float,
      VertexType.Double        => VertexAttribPointerType.Double,

      _ => throw new ArgumentOutOfRangeException(nameof(attributeType), attributeType, null)
    };
  }

  private static PrimitiveType ConvertMeshType(MeshType meshType)
  {
    return meshType switch
    {
      MeshType.Points    => PrimitiveType.Points,
      MeshType.Lines     => PrimitiveType.Lines,
      MeshType.LineStrip => PrimitiveType.LineStrip,
      MeshType.LineLoop  => PrimitiveType.LineLoop,
      MeshType.Triangles => PrimitiveType.Triangles,
      MeshType.Quads     => PrimitiveType.Quads,
      MeshType.QuadStrip => PrimitiveType.QuadStrip,

      _ => throw new ArgumentOutOfRangeException(nameof(meshType), meshType, null)
    };
  }
}
