using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.EXT;
using Surreal.Collections;
using Surreal.Collections.Slices;
using Surreal.Colors;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using PolygonMode = Surreal.Graphics.Materials.PolygonMode;
using ShaderType = Surreal.Graphics.Materials.ShaderType;
using TextureWrapMode = Surreal.Graphics.Textures.TextureWrapMode;

namespace Surreal.Graphics;

/// <summary>
/// A <see cref="IGraphicsDevice"/> implementation that uses OpenGL via Silk.NET.
/// </summary>
internal sealed class SilkGraphicsDeviceOpenGL(GL gl) : IGraphicsDevice
{
  private readonly bool _isMarkersAvailable = gl.IsExtensionPresent("GL_EXT_debug_marker");
  private readonly ExtDebugMarker _debugMarker = new(gl.Context);
  private readonly Stack<FrameBufferHandle> _activeFrameBuffers = new();

  private Mesh? _quadMesh;
  private FrameBufferHandle _activeFrameBuffer;

  public Viewport GetViewportSize()
  {
    Span<int> size = stackalloc int[4];

    gl.GetInteger(GetPName.Viewport, size);

    return new Viewport(size[0], size[1], (uint)size[2], (uint)size[3]);
  }

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

  public void SetScissorState(ScissorState? state)
  {
    if (state != null)
    {
      gl.Enable(EnableCap.ScissorTest);

      gl.Scissor(
        x: state.Value.Left,
        y: state.Value.Top,
        width: (uint)(state.Value.Right - state.Value.Left),
        height: (uint)(state.Value.Bottom - state.Value.Top)
      );
    }
    else
    {
      gl.Disable(EnableCap.ScissorTest);
    }
  }

  public void SetPolygonMode(PolygonMode mode)
  {
    gl.PolygonMode(TriangleFace.FrontAndBack, mode switch
    {
      PolygonMode.Filled => GLEnum.Fill,
      PolygonMode.Lines => GLEnum.Line,

      _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
    });
  }

  public void SetCullingMode(CullingMode mode)
  {
    if (mode != CullingMode.Disabled)
    {
      gl.Enable(EnableCap.CullFace);

      gl.CullFace(mode switch
      {
        CullingMode.Front => TriangleFace.Front,
        CullingMode.Back => TriangleFace.Back,
        CullingMode.Both => TriangleFace.FrontAndBack,

        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
      });
    }
    else
    {
      gl.Disable(GLEnum.CullFace);
    }
  }

  public void ClearColorBuffer(Color color)
  {
    gl.ClearColor(color.R, color.G, color.B, color.A);
    gl.Clear(ClearBufferMask.ColorBufferBit);
  }

  public void ClearDepthBuffer(float depth)
  {
    gl.ClearDepth(depth);
    gl.Clear(ClearBufferMask.DepthBufferBit);
  }

  public void ClearStencilBuffer(int amount)
  {
    gl.ClearStencil(amount);
    gl.Clear(ClearBufferMask.StencilBufferBit);
  }

  public GraphicsHandle CreatePipeline(PipelineDescriptor descriptor)
  {
    throw new NotImplementedException();
  }

  public void DeletePipeline(GraphicsHandle handle)
  {
    throw new NotImplementedException();
  }

  public GraphicsHandle CreateBuffer()
  {
    return GraphicsHandle.FromUInt(gl.GenBuffer());
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

  public unsafe void ReadBufferData<T>(GraphicsHandle handle, BufferType type, Span<T> buffer) where T : unmanaged
  {
    var kind = ConvertBufferType(type);

    gl.BindBuffer(kind, handle);

    fixed (T* pointer = buffer)
    {
      gl.GetBufferSubData(
        target: kind,
        offset: 0,
        size: (uint)(buffer.Length * sizeof(T)),
        data: pointer
      );
    }
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

    return GraphicsHandle.FromUInt(texture);
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

  public unsafe void WriteTextureSubData<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, ReadOnlySpan<T> pixels, int mipLevel = 0)
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

    return GraphicsHandle.FromUInt(array);
  }

  public unsafe void DrawMesh(GraphicsHandle mesh, uint vertexCount, uint indexCount, MeshType meshType, Type indexType)
  {
    gl.BindVertexArray(mesh);

    var primitiveType = ConvertMeshType(meshType);

    if (indexCount > 0)
    {
      var elementType = ConvertElementType(indexType);

      gl.DrawElements(primitiveType, indexCount, elementType, null);
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
    return GraphicsHandle.FromUInt(gl.CreateProgram());
  }

  public void LinkShader(GraphicsHandle handle, ReadOnlySlice<ShaderKernel> kernels)
  {
    var shaderIds = new List<uint>();

    foreach (var kernel in kernels)
    {
      var shader = gl.CreateShader(ConvertShaderType(kernel.Type));

      shaderIds.Add(shader);

      gl.ShaderSource(shader, kernel.Code.ToString());
      gl.CompileShader(shader);

      if (gl.GetShader(shader, ShaderParameterName.CompileStatus) == 0)
      {
        var infoLog = gl.GetShaderInfoLog(shader);

        throw new InvalidOperationException($"Failed to compile shader: {infoLog}");
      }

      gl.AttachShader(handle, shader);
    }

    gl.LinkProgram(handle);

    if (gl.GetProgram(handle, ProgramPropertyARB.LinkStatus) == 0)
    {
      var infoLog = gl.GetProgramInfoLog(handle);

      throw new InvalidOperationException($"Failed to link shader: {infoLog}");
    }

    foreach (var shader in shaderIds)
    {
      gl.DetachShader(handle, shader);
      gl.DeleteShader(shader);
    }
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

  public void SetShaderUniform(GraphicsHandle handle, int location, double value)
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

  public void SetShaderUniform(GraphicsHandle handle, int location, Color value)
  {
    gl.ProgramUniform4(handle, location, value.R, value.G, value.B, value.A);
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, Color32 value)
  {
    gl.ProgramUniform4(handle, location, value.R, value.G, value.B, value.A);
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

  public void SetShaderSampler(GraphicsHandle handle, int location, GraphicsHandle texture, uint samplerSlot)
  {
    gl.ActiveTexture(TextureUnit.Texture0 + (int)samplerSlot);
    gl.BindTexture(TextureTarget.Texture2D, texture);
    gl.ProgramUniform1(handle, location, samplerSlot);
  }

  public void SetShaderSampler(GraphicsHandle handle, int location, TextureSampler sampler)
  {
    gl.ActiveTexture(TextureUnit.Texture0 + (int)sampler.SamplerSlot);
    gl.BindTexture(TextureTarget.Texture2D, sampler.Texture);
    gl.ProgramUniform1(handle, location, sampler.SamplerSlot);

    // apply sampler settings
    if (sampler.FilterMode.HasValue || sampler.WrapMode.HasValue)
    {
      var samplerId = gl.CreateSampler();

      if (sampler.FilterMode.HasValue)
      {
        gl.SamplerParameterI(samplerId, SamplerParameterI.MinFilter, ConvertTextureFilterMode(sampler.FilterMode.Value));
      }

      if (sampler.WrapMode.HasValue)
      {
        gl.SamplerParameterI(samplerId, SamplerParameterI.WrapS, ConvertTextureWrapMode(sampler.WrapMode.Value));
        gl.SamplerParameterI(samplerId, SamplerParameterI.WrapT, ConvertTextureWrapMode(sampler.WrapMode.Value));
      }

      gl.BindSampler(sampler.SamplerSlot, samplerId);
    }
    else
    {
      gl.BindSampler(sampler.SamplerSlot, 0);
    }
  }

  void IGraphicsDevice.SetActiveShader(GraphicsHandle handle)
  {
    gl.UseProgram(handle);
  }

  public void DeleteShader(GraphicsHandle handle)
  {
    gl.DeleteProgram(handle);
  }

  public unsafe FrameBufferHandle CreateFrameBuffer(RenderTargetDescriptor descriptor)
  {
    var framebuffer = gl.GenFramebuffer();
    var viewportSize = GetViewportSize();

    gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

    // build the color attachment
    var colorAttachment = gl.GenTexture();
    var depthStencilAttachment = 0u;

    gl.BindTexture(TextureTarget.Texture2D, colorAttachment);
    gl.TexImage2D(
      target: TextureTarget.Texture2D,
      level: 0,
      internalformat: GetInternalFormat(descriptor.Format),
      width: descriptor.Width.GetOrDefault(viewportSize.Width),
      height: descriptor.Height.GetOrDefault(viewportSize.Height),
      border: 0,
      format: PixelFormat.Rgb,
      type: PixelType.UnsignedByte,
      pixels: null
    );

    gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ConvertTextureFilterMode(descriptor.FilterMode));
    gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ConvertTextureFilterMode(descriptor.FilterMode));
    gl.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, colorAttachment, level: 0);

    // build the depth attachment
    if (descriptor.DepthStencilFormat != DepthStencilFormat.None)
    {
      depthStencilAttachment = gl.GenFramebuffer();

      gl.BindRenderbuffer(GLEnum.Renderbuffer, depthStencilAttachment);

      gl.RenderbufferStorage(
        target: RenderbufferTarget.Renderbuffer,
        internalformat: ConvertDepthStencilFormat(descriptor.DepthStencilFormat),
        width: descriptor.Width.GetOrDefault(viewportSize.Width),
        height: descriptor.Height.GetOrDefault(viewportSize.Height)
      );

      gl.FramebufferRenderbuffer(
        target: FramebufferTarget.Framebuffer,
        attachment: FramebufferAttachment.DepthStencilAttachment,
        renderbuffertarget: RenderbufferTarget.Renderbuffer,
        renderbuffer: depthStencilAttachment
      );
    }

    gl.DrawBuffers(stackalloc GLEnum[] { GLEnum.ColorAttachment0 });

    if (gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
    {
      throw new InvalidOperationException("Failed to create framebuffer");
    }

    gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

    return new FrameBufferHandle
    {
      FrameBuffer = GraphicsHandle.FromUInt(framebuffer),
      ColorAttachment = GraphicsHandle.FromUInt(colorAttachment),
      DepthStencilAttachment = GraphicsHandle.FromUInt(depthStencilAttachment)
    };
  }

  public bool IsActiveFrameBuffer(FrameBufferHandle handle)
  {
    return handle == _activeFrameBuffer;
  }

  public void BindFrameBuffer(FrameBufferHandle handle)
  {
    gl.BindFramebuffer(FramebufferTarget.Framebuffer, handle.FrameBuffer);

    _activeFrameBuffers.Push(_activeFrameBuffer);
    _activeFrameBuffer = handle;
  }

  public void UnbindFrameBuffer()
  {
    if (_activeFrameBuffers.TryPop(out var handle))
    {
      gl.BindFramebuffer(FramebufferTarget.Framebuffer, handle.FrameBuffer);

      _activeFrameBuffer = handle;
    }
    else
    {
      gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
  }

  public unsafe void ResizeFrameBuffer(FrameBufferHandle handle, uint width, uint height)
  {
    gl.BindTexture(TextureTarget.Texture2D, handle.ColorAttachment);

    gl.TexImage2D(
      target: TextureTarget.Texture2D,
      level: 0,
      internalformat: GetInternalFormat(TextureFormat.Rgba8),
      width: width,
      height: height,
      border: 0,
      format: PixelFormat.Rgba,
      type: PixelType.UnsignedByte,
      pixels: null
    );

    if (handle.DepthStencilAttachment != GraphicsHandle.None)
    {
      gl.BindRenderbuffer(GLEnum.Renderbuffer, handle.DepthStencilAttachment);

      gl.RenderbufferStorage(
        target: RenderbufferTarget.Renderbuffer,
        internalformat: ConvertDepthStencilFormat(DepthStencilFormat.Depth24Stencil8),
        width: width,
        height: height
      );
    }
  }

  public void BlitFromFrameBuffer(GraphicsHandle targetFrameBuffer, uint sourceWidth, uint sourceHeight, uint destWidth, uint destHeight, BlitMask mask, TextureFilterMode filterMode)
  {
    gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
    gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, targetFrameBuffer);

    gl.BlitFramebuffer(
      srcX0: 0,
      srcY0: 0,
      srcX1: (int)sourceWidth,
      srcY1: (int)sourceHeight,
      dstX0: 0,
      dstY0: 0,
      dstX1: (int)destWidth,
      dstY1: (int)destHeight,
      mask: ConvertBlitMask(mask),
      filter: filterMode switch
      {
        TextureFilterMode.Linear => BlitFramebufferFilter.Linear,
        TextureFilterMode.Point => BlitFramebufferFilter.Nearest,

        _ => throw new ArgumentOutOfRangeException(nameof(filterMode), filterMode, null)
      }
    );

    gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
    gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
  }

  public void BlitToFrameBuffer(GraphicsHandle sourceFrameBuffer, uint sourceWidth, uint sourceHeight, uint destWidth, uint destHeight, BlitMask mask, TextureFilterMode filterMode)
  {
    gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, sourceFrameBuffer);
    gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

    gl.BlitFramebuffer(
      srcX0: 0,
      srcY0: 0,
      srcX1: (int)sourceWidth,
      srcY1: (int)sourceHeight,
      dstX0: 0,
      dstY0: 0,
      dstX1: (int)destWidth,
      dstY1: (int)destHeight,
      mask: ConvertBlitMask(mask),
      filter: filterMode switch
      {
        TextureFilterMode.Linear => BlitFramebufferFilter.Linear,
        TextureFilterMode.Point => BlitFramebufferFilter.Nearest,

        _ => throw new ArgumentOutOfRangeException(nameof(filterMode), filterMode, null)
      }
    );

    gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
    gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
  }

  public void BlitToFrameBuffer(
    FrameBufferHandle sourceFrameBuffer,
    Material material,
    ShaderProperty<TextureSampler> samplerProperty,
    Optional<TextureFilterMode> filterMode,
    Optional<TextureWrapMode> wrapMode)
  {
    var mesh = GetOrCreateQuadMesh();

    var sampler = new TextureSampler(sourceFrameBuffer.ColorAttachment, 0)
    {
      FilterMode = filterMode,
      WrapMode = wrapMode
    };

    material.ApplyMaterial();
    material.Uniforms.Set(samplerProperty, sampler);

    mesh.Draw(material);
  }

  public void DeleteFrameBuffer(FrameBufferHandle handle)
  {
    gl.DeleteFramebuffer(handle.FrameBuffer);
    gl.DeleteTexture(handle.ColorAttachment);

    if (handle.DepthStencilAttachment != GraphicsHandle.None)
    {
      gl.DeleteRenderbuffer(handle.DepthStencilAttachment);
    }
  }

  public void BeginDebugScope(string name)
  {
    if (_isMarkersAvailable)
    {
      _debugMarker.PushGroupMarker((uint)name.Length, name);
    }
  }

  public void EndDebugScope()
  {
    if (_isMarkersAvailable)
    {
      _debugMarker.PopGroupMarker();
    }
  }

  /// <summary>
  /// Gets or creates a fullscreen quad mesh.
  /// </summary>
  private Mesh GetOrCreateQuadMesh()
  {
    return _quadMesh ??= Mesh.CreateQuad(this);
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
      TextureWrapMode.ClampToEdge => (int)GLEnum.ClampToEdge,
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

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static GLEnum ConvertDepthStencilFormat(DepthStencilFormat format)
  {
    return format switch
    {
      DepthStencilFormat.None => GLEnum.None,
      DepthStencilFormat.Depth24 => GLEnum.DepthComponent24,
      DepthStencilFormat.Depth24Stencil8 => GLEnum.Depth24Stencil8,
      DepthStencilFormat.Depth32Stencil8 => GLEnum.Depth32fStencil8,

      _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
    };
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static GLEnum ConvertShaderType(ShaderType type)
  {
    return type switch
    {
      ShaderType.VertexShader => GLEnum.VertexShader,
      ShaderType.FragmentShader => GLEnum.FragmentShader,

      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
  }

  private static uint ConvertBlitMask(BlitMask mask)
  {
    var result = ClearBufferMask.None;

    if (mask.HasFlagFast(BlitMask.Color)) result |= ClearBufferMask.ColorBufferBit;
    if (mask.HasFlagFast(BlitMask.Depth)) result |= ClearBufferMask.DepthBufferBit;
    if (mask.HasFlagFast(BlitMask.Stencil)) result |= ClearBufferMask.StencilBufferBit;

    return (uint)result;
  }

  public void Dispose()
  {
    _debugMarker.Dispose();
    _quadMesh?.Dispose();

    gl.Dispose();
  }
}
