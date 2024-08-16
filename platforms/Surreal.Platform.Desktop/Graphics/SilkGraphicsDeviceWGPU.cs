using Silk.NET.Core.Native;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using Surreal.Collections.Slices;
using Surreal.Colors;
using Surreal.Diagnostics.Logging;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using BlendState = Surreal.Graphics.Materials.BlendState;
using Buffer = Silk.NET.WebGPU.Buffer;
using BufferUsageFlags = Silk.NET.WebGPU.BufferUsage;
using BufferUsage = Surreal.Graphics.Meshes.BufferUsage;
using Color = Surreal.Colors.Color;
using PolygonMode = Surreal.Graphics.Materials.PolygonMode;
using Queue = Silk.NET.WebGPU.Queue;
using RenderPipeline = Silk.NET.WebGPU.RenderPipeline;
using TextureFormat = Surreal.Graphics.Textures.TextureFormat;

namespace Surreal.Graphics;

/// <summary>
/// A <see cref="IGraphicsDevice"/> implementation that uses WGPU via Silk.NET.
/// </summary>
internal sealed unsafe class SilkGraphicsDeviceWGPU : IGraphicsDevice
{
  private static readonly ILog Log = LogFactory.GetLog<SilkGraphicsDeviceWGPU>();

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  private readonly WebGPU wgpu = WebGPU.GetApi();

  private readonly Instance* _instance;
  private readonly Surface* _surface;
  private Adapter* _adapter;
  private Device* _device;
  private Queue* _queue;

  public SilkGraphicsDeviceWGPU(IWindow window, GraphicsMode mode)
  {
    var instanceDescriptor = new InstanceDescriptor();

    _instance = wgpu.CreateInstance(in instanceDescriptor);
    _surface = window.CreateWebGPUSurface(wgpu, _instance);

    var requestAdapterOptions = new RequestAdapterOptions
    {
      PowerPreference = mode switch
      {
        GraphicsMode.Universal => PowerPreference.LowPower,
        GraphicsMode.HighFidelity => PowerPreference.HighPerformance,

        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
      }
    };

    wgpu.InstanceRequestAdapter(
      instance: _instance,
      options: &requestAdapterOptions,
      callback: new PfnRequestAdapterCallback((_, adapter, _, _) => _adapter = adapter),
      userdata: null
    );

    var deviceDescriptor = new DeviceDescriptor
    {
      DeviceLostCallback = new PfnDeviceLostCallback(OnDeviceLost)
    };

    wgpu.AdapterRequestDevice(
      adapter: _adapter,
      descriptor: &deviceDescriptor,
      callback: new PfnRequestDeviceCallback((_, device, _, _) => _device = device),
      userdata: null
    );

    wgpu.DeviceSetUncapturedErrorCallback(
      device: _device,
      callback: new PfnErrorCallback(OnUnhandledError),
      userdata: null
    );

    _queue = wgpu.DeviceGetQueue(_device);
  }

  public Viewport GetViewportSize()
  {
    return new Viewport();
  }

  public void SetViewportSize(Viewport viewport)
  {
  }

  public void SetBlendState(BlendState? state)
  {
  }

  public void SetScissorState(ScissorState? state)
  {
  }

  public void SetPolygonMode(PolygonMode mode)
  {
  }

  public void SetCullingMode(CullingMode mode)
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

  public GraphicsHandle CreateBuffer(BufferType type, BufferUsage usage)
  {
    var descriptor = new BufferDescriptor
    {
      Usage = type switch
      {
        BufferType.Vertex => BufferUsageFlags.Vertex | BufferUsageFlags.CopyDst | BufferUsageFlags.CopySrc,
        BufferType.Index => BufferUsageFlags.Index | BufferUsageFlags.CopyDst | BufferUsageFlags.CopySrc,

        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      },
    };

    var buffer = wgpu.DeviceCreateBuffer(_device, &descriptor);

    return GraphicsHandle.FromPointer(buffer);
  }

  public GraphicsTask<Memory<T>> ReadBufferDataAsync<T>(GraphicsHandle handle, BufferType type) where T : unmanaged
  {
    var task = GraphicsTask.Create<Memory<T>>();
    var buffer = handle.AsPointer<Buffer>();
    var size = (uint)wgpu.BufferGetSize(buffer);

    var memory = new Memory<T>(GC.AllocateArray<T>((int)size));

    wgpu.BufferMapAsync(
      buffer: buffer,
      mode: MapMode.Read,
      offset: 0,
      size: size,
      callback: new PfnBufferMapCallback((status, result) =>
      {
        if (status == BufferMapAsyncStatus.Success)
        {
          fixed (T* pointer = memory.Span)
          {
            Unsafe.CopyBlock(pointer, result, size);
          }

          task.SignalCompletion(memory);
        }
        else
          task.SignalException(new Exception("Failed to read buffer data."));
      }),
      userdata: null
    );

    return task;
  }

  public GraphicsTask WriteBufferDataAsync<T>(GraphicsHandle handle, BufferType type, ReadOnlySpan<T> span, BufferUsage usage) where T : unmanaged
  {
    // TODO: how to get the right size in advance?
    var task = GraphicsTask.Create();
    var buffer = handle.AsPointer<Buffer>();

    fixed (T* pointer = span)
    {
      var size = (nuint)(span.Length * sizeof(T));

      wgpu.QueueWriteBuffer(_queue, buffer, 0, pointer, size);
      wgpu.QueueOnSubmittedWorkDone(
        queue: _queue,
        callback: new PfnQueueWorkDoneCallback((status, _) =>
        {
          if (status == QueueWorkDoneStatus.Success)
            task.SignalCompletion();
          else
            task.SignalException(new Exception("Failed to write buffer sub data."));
        }),
        userdata: null
      );
    }

    return task;
  }

  public GraphicsTask WriteBufferDataAsync<T>(GraphicsHandle handle, BufferType type, uint offset, ReadOnlySpan<T> span) where T : unmanaged
  {
    var task = GraphicsTask.Create();
    var buffer = handle.AsPointer<Buffer>();

    fixed (T* pointer = span)
    {
      var size = (nuint)(span.Length * sizeof(T));

      wgpu.QueueWriteBuffer(_queue, buffer, offset, pointer, size);
      wgpu.QueueOnSubmittedWorkDone(
        queue: _queue,
        callback: new PfnQueueWorkDoneCallback((status, _) =>
        {
          if (status == QueueWorkDoneStatus.Success)
            task.SignalCompletion();
          else
            task.SignalException(new Exception("Failed to write buffer sub data."));
        }),
        userdata: null
      );
    }

    return task;
  }

  public void DeleteBuffer(GraphicsHandle handle)
  {
    wgpu.BufferRelease(handle.AsPointer<Buffer>());
  }

  public GraphicsHandle CreateTexture(TextureFilterMode filterMode, TextureWrapMode wrapMode)
  {
    var state = new TextureState
    {
      FilterMode = filterMode,
      WrapMode = wrapMode
    };

    return GraphicsHandle.FromObject(state);
  }

  public GraphicsTask<Memory<T>> ReadTextureDataAsync<T>(GraphicsHandle handle, int mipLevel = 0) where T : unmanaged
  {
    return GraphicsTask.FromResult(Memory<T>.Empty);
  }

  public GraphicsTask<Memory<T>> ReadTextureDataAsync<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, int mipLevel = 0) where T : unmanaged
  {
    return GraphicsTask.FromResult(Memory<T>.Empty);
  }

  public GraphicsTask WriteTextureDataAsync<T>(GraphicsHandle handle, uint width, uint height, ReadOnlySpan<T> pixels, TextureFormat format, int mipLevel = 0) where T : unmanaged
  {
    return GraphicsTask.CompletedTask;
  }

  public GraphicsTask WriteTextureDataAsync<T>(GraphicsHandle handle, int offsetX, int offsetY, uint width, uint height, ReadOnlySpan<T> pixels, int mipLevel = 0) where T : unmanaged
  {
    return GraphicsTask.CompletedTask;
  }

  public void SetTextureFilterMode(GraphicsHandle handle, TextureFilterMode mode)
  {
    var state = handle.AsObject<TextureState>();

    state.FilterMode = mode;
  }

  public void SetTextureWrapMode(GraphicsHandle handle, TextureWrapMode mode)
  {
    var state = handle.AsObject<TextureState>();

    state.WrapMode = mode;
  }

  public void DeleteTexture(GraphicsHandle handle)
  {
    handle.Dispose();
  }

  public GraphicsHandle CreateMesh(GraphicsHandle vertices, GraphicsHandle indices, VertexDescriptorSet descriptors)
  {
    return GraphicsHandle.None;
  }

  public void DrawMesh(GraphicsHandle mesh, uint vertexCount, uint indexCount, MeshType meshType, Type indexType)
  {
  }

  public void DeleteMesh(GraphicsHandle handle)
  {
  }

  public GraphicsHandle CreateShader()
  {
    return GraphicsHandle.None;
  }

  public void LinkShader(GraphicsHandle handle, ReadOnlySlice<ShaderKernel> kernels)
  {
  }

  public int GetShaderUniformLocation(GraphicsHandle handle, string name)
  {
    return -1;
  }

  public void SetShaderUniform(GraphicsHandle handle, int location, int value)
  {
    var shader = handle.AsObject<ShaderState>();
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

  public void SetShaderSampler(GraphicsHandle handle, int location, GraphicsHandle texture, uint samplerSlot)
  {
  }

  public void SetShaderSampler(GraphicsHandle handle, int location, TextureSampler sampler)
  {
  }

  public void SetActiveShader(GraphicsHandle handle)
  {
  }

  public void DeleteShader(GraphicsHandle handle)
  {
  }

  public FrameBufferHandle CreateFrameBuffer(RenderTargetDescriptor descriptor)
  {
    return FrameBufferHandle.None;
  }

  public bool IsActiveFrameBuffer(FrameBufferHandle handle)
  {
    return true;
  }

  public void BindFrameBuffer(FrameBufferHandle handle)
  {
  }

  public void UnbindFrameBuffer()
  {
  }

  public void ResizeFrameBuffer(FrameBufferHandle handle, uint width, uint height)
  {
  }

  public void BlitFromFrameBuffer(GraphicsHandle targetFrameBuffer, uint sourceWidth, uint sourceHeight, uint destWidth, uint destHeight, BlitMask mask, TextureFilterMode filterMode)
  {
  }

  public void BlitToFrameBuffer(GraphicsHandle sourceFrameBuffer, uint sourceWidth, uint sourceHeight, uint destWidth, uint destHeight, BlitMask mask, TextureFilterMode filterMode)
  {
  }

  public void BlitToFrameBuffer(FrameBufferHandle sourceFrameBuffer, Material material, ShaderProperty<TextureSampler> samplerProperty, Optional<TextureFilterMode> filterMode, Optional<TextureWrapMode> wrapMode)
  {
  }

  public void DeleteFrameBuffer(FrameBufferHandle handle)
  {
  }

  public void BeginDebugScope(string name)
  {
  }

  public void EndDebugScope()
  {
  }

  public void Dispose()
  {
    wgpu.QueueRelease(_queue);
    wgpu.DeviceRelease(_device);
    wgpu.AdapterRelease(_adapter);
    wgpu.SurfaceRelease(_surface);
    wgpu.InstanceRelease(_instance);

    wgpu.Dispose();
  }

  /// <summary>
  /// A callback for when the device is lost.
  /// </summary>
  private static void OnDeviceLost(DeviceLostReason reason, byte* message, void* userData)
  {
    Log.Warn($"Device lost {SilkMarshal.PtrToString((nint)message)}");
  }

  /// <summary>
  /// A callback for when an error is not captured.
  /// </summary>
  private static void OnUnhandledError(ErrorType arg0, byte* message, void* userData)
  {
    Log.Error($"Unhandled error {SilkMarshal.PtrToString((nint)message)}");
  }

  /// <summary>
  /// Internal state for a buffer.
  /// </summary>
  private sealed class BufferState
  {
    public required Buffer* Buffer { get; init; }
  }

  /// <summary>
  /// Internal state for a pipeline.
  /// </summary>
  private sealed class PipelineState
  {
    public required RenderPipeline* Pipeline { get; init; }
  }

  /// <summary>
  /// Internal state for a shader.
  /// </summary>
  private sealed class ShaderState
  {
    public required ShaderModule* VertexModule { get; init; }
    public required ShaderModule* FragmentModule { get; init; }
  }

  /// <summary>
  /// Internal state for a texture.
  /// </summary>
  private sealed class TextureState
  {
    public TextureFilterMode FilterMode { get; set; }
    public TextureWrapMode WrapMode { get; set; }
  }
}
