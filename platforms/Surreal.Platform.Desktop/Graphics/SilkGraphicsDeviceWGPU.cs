using Silk.NET.Core.Native;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using Surreal.Collections;
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
  private readonly Queue* _queue;

  private readonly Arena<BufferState> _buffers = [];
  private readonly Arena<TextureState> _textures = [];

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
        BufferType.Vertex => BufferUsageFlags.Vertex | BufferUsageFlags.CopyDst,
        BufferType.Index => BufferUsageFlags.Index | BufferUsageFlags.CopyDst,

        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      },
    };

    var state = new BufferState
    {
      Buffer = wgpu.DeviceCreateBuffer(_device, &descriptor)
    };

    return GraphicsHandle.FromArenaIndex(_buffers.Add(state));
  }

  public GraphicsTask<Memory<T>> ReadBufferDataAsync<T>(GraphicsHandle handle, BufferType type) where T : unmanaged
  {
    var task = GraphicsTask.Create<Memory<T>>();
    var state = _buffers[handle];

    var size = (uint)wgpu.BufferGetSize(state.Buffer);
    var memory = new Memory<T>(GC.AllocateArray<T>((int)(size / sizeof(T))));

    wgpu.BufferMapAsync(
      buffer: state.Buffer,
      mode: MapMode.Read,
      offset: 0,
      size: size,
      callback: new PfnBufferMapCallback((status, result) =>
      {
        if (status == BufferMapAsyncStatus.Success)
        {
          fixed (T* pointer = memory.Span)
            Unsafe.CopyBlock(pointer, result, size);

          task.SignalCompletion(memory);
        }
        else
          task.SignalException(new Exception($"Failed to read buffer data with reason {status}"));
      }),
      userdata: null
    );

    wgpu.QueueSubmit(_queue, 0, null);

    return task;
  }

  public GraphicsTask WriteBufferDataAsync<T>(GraphicsHandle handle, BufferType type, ReadOnlySpan<T> span, BufferUsage usage) where T : unmanaged
  {
    return WriteBufferDataAsync(handle, type, 0, span, usage);
  }

  public GraphicsTask WriteBufferDataAsync<T>(GraphicsHandle handle, BufferType type, uint offset, ReadOnlySpan<T> span, BufferUsage usage) where T : unmanaged
  {
    var task = GraphicsTask.Create();
    var state = _buffers[handle];

    var currentSize = wgpu.BufferGetSize(state.Buffer);
    var targetSize = (nuint)(span.Length * sizeof(T));

    // resize the buffer if it's too small
    if (currentSize < targetSize)
    {
      wgpu.BufferRelease(state.Buffer);

      var descriptor = new BufferDescriptor
      {
        Usage = type switch
        {
          BufferType.Vertex => BufferUsageFlags.Vertex | BufferUsageFlags.CopyDst,
          BufferType.Index => BufferUsageFlags.Index | BufferUsageFlags.CopyDst,

          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        },
        Size = targetSize
      };

      state.Buffer = wgpu.DeviceCreateBuffer(_device, &descriptor);
    }

    // enqueue to write
    fixed (T* pointer = span)
    {
      wgpu.QueueWriteBuffer(_queue, state.Buffer, offset, pointer, targetSize);
    }

    // submit the work
    wgpu.QueueOnSubmittedWorkDone(
      queue: _queue,
      callback: new PfnQueueWorkDoneCallback((status, _) =>
      {
        if (status == QueueWorkDoneStatus.Success)
          task.SignalCompletion();
        else
          task.SignalException(new Exception($"Failed to write buffer sub data with reason {status}"));
      }),
      userdata: null
    );

    wgpu.QueueSubmit(_queue, 0, null);

    return task;
  }

  public void DeleteBuffer(GraphicsHandle handle)
  {
    if (_buffers.TryRemove(handle, out var state))
    {
      wgpu.BufferRelease(state.Buffer);
    }
  }

  public GraphicsHandle CreateTexture(TextureFilterMode filterMode, TextureWrapMode wrapMode)
  {
    var state = new TextureState
    {
      FilterMode = filterMode,
      WrapMode = wrapMode
    };

    return GraphicsHandle.FromArenaIndex(_textures.Add(state));
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
    var state = _textures[handle];

    state.FilterMode = mode;
  }

  public void SetTextureWrapMode(GraphicsHandle handle, TextureWrapMode mode)
  {
    var state = _textures[handle];

    state.WrapMode = mode;
  }

  public void DeleteTexture(GraphicsHandle handle)
  {
    _textures.Remove(handle);
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

  public GraphicsTask BlitFromDisplayFrameBufferAsync(GraphicsHandle targetBuffer, uint sourceWidth, uint sourceHeight, uint destWidth, uint destHeight, BlitMask mask, TextureFilterMode filterMode)
  {
    return GraphicsTask.CompletedTask;
  }

  public GraphicsTask BlitToTargetFrameBufferAsync(FrameBufferHandle sourceBuffer, FrameBufferHandle targetBuffer, Material material, ShaderProperty<TextureSampler> samplerProperty, Optional<TextureFilterMode> filterMode, Optional<TextureWrapMode> wrapMode)
  {
    return GraphicsTask.CompletedTask;
  }

  public GraphicsTask BlitToDisplayFrameBufferAsync(GraphicsHandle sourceBuffer, uint sourceWidth, uint sourceHeight, uint destWidth, uint destHeight, BlitMask mask, TextureFilterMode filterMode)
  {
    return GraphicsTask.CompletedTask;
  }

  public GraphicsTask BlitToDisplayFrameBufferAsync(FrameBufferHandle sourceBuffer, Material material, ShaderProperty<TextureSampler> samplerProperty, Optional<TextureFilterMode> filterMode, Optional<TextureWrapMode> wrapMode)
  {
    return GraphicsTask.CompletedTask;
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
    public required Buffer* Buffer { get; set; }
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
