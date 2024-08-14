using Surreal.Audio;
using Surreal.Diagnostics;
using Surreal.Diagnostics.Logging;
using Surreal.Graphics;
using Surreal.Graphics.Images;
using Surreal.Input;
using Surreal.Services;
using Surreal.Timing;

namespace Surreal;

/// <summary>
/// A specialization of <see cref="IPlatformHost" /> for desktop environments.
/// </summary>
public interface IDesktopPlatformHost : IPlatformHost
{
  IDesktopWindow PrimaryWindow { get; }
}

/// <summary>
/// Allows access to the platform's window.
/// </summary>
public interface IDesktopWindow : IDisposable
{
  event Action<int, int> Resized;

  string Title { get; set; }

  int Width { get; set; }
  int Height { get; set; }

  bool IsVisible { get; set; }
  bool IsVsyncEnabled { get; set; }
  bool IsEventDriven { get; set; }
  bool IsFocused { get; }
  bool IsClosing { get; }

  /// <summary>
  /// Sets the window icon to the given image.
  /// </summary>
  void SetWindowIcon(Image image);
}

internal sealed class DesktopPlatformHost : IDesktopPlatformHost
{
  private static readonly ILog Log = LogFactory.GetLog<DesktopPlatformHost>();

  private readonly ThreadAffineSynchronizationContext _syncContext = new(Thread.CurrentThread);
  private readonly FrameCounter _frameCounter = new();
  private readonly DesktopConfiguration _configuration;

  private IntervalTimer _frameDisplayTimer = new(TimeSpan.FromSeconds(1));

  public DesktopPlatformHost(DesktopConfiguration configuration)
  {
    SynchronizationContext.SetSynchronizationContext(_syncContext);

    _configuration = configuration;

    Window = new SilkWindow(configuration);
    AudioBackend = new SilkAudioBackend();
    GraphicsBackend = new SilkGraphicsBackend(Window.OpenGL);
    InputBackend = new SilkInputBackend(Window.InnerWindow, Window.Input);

    Resized += OnResized;
  }

  public SilkWindow Window { get; }
  public SilkAudioBackend AudioBackend { get; }
  public SilkGraphicsBackend GraphicsBackend { get; }
  public SilkInputBackend InputBackend { get; }

  IDesktopWindow IDesktopPlatformHost.PrimaryWindow => Window;

  public event Action<int, int> Resized
  {
    add => Window.Resized += value;
    remove => Window.Resized -= value;
  }

  public int Width => Window.Width;
  public int Height => Window.Height;

  public bool IsFocused => Window.IsFocused;
  public bool IsClosing => Window.IsClosing;
  public bool IsVisible => Window.IsVisible;

  public void RegisterServices(IServiceRegistry services)
  {
    services.AddService<IDesktopPlatformHost>(this);
    services.AddService<IDesktopWindow>(Window);
    services.AddService<IAudioBackend>(AudioBackend);
    services.AddService<IGraphicsBackend>(GraphicsBackend);
    services.AddService<IInputBackend>(InputBackend);
  }

  public void BeginFrame(DeltaTime deltaTime)
  {
    _syncContext.Process();

    if (!IsClosing)
    {
      Window.Update();

      // show the game's FPS in the window title
      if (_configuration.ShowFpsInTitle)
      {
        _frameCounter.Tick(deltaTime);

        if (_frameDisplayTimer.Tick(deltaTime))
        {
          Window.Title = $"{_configuration.Title} - {_frameCounter.TicksPerSecond:F} FPS";
        }
      }
    }
  }

  public void EndFrame(DeltaTime deltaTime)
  {
    if (!IsClosing)
    {
      Window.Present();

      InputBackend.Update();
    }
  }

  public void Dispose()
  {
    Window.Dispose();
  }

  private void OnResized(int width, int height)
  {
    Log.Trace($"Resizing window to {width}x{height}");

    // TODO: Implement this
    // GraphicsBackend.SetViewportSize(new Viewport(0, 0, (uint)width, (uint)height));
  }
}

/// <summary>
/// A <see cref="SynchronizationContext"/> that prefers to schedule work back onto the main thread.
/// </summary>
internal sealed class ThreadAffineSynchronizationContext(Thread mainThread) : SynchronizationContext
{
  private readonly Queue<Continuation> _continuations = new();
  private readonly Queue<Continuation> _buffer = new();

  public void Process()
  {
    if (Thread.CurrentThread != mainThread)
    {
      throw new InvalidOperationException("Cannot process continuations from a non-main thread.");
    }

    while (_continuations.TryDequeue(out var continuation))
    {
      _buffer.Enqueue(continuation);
    }

    while (_buffer.TryDequeue(out var continuation))
    {
      continuation.Execute();
    }
  }

  public override void Post(SendOrPostCallback callback, object? state)
  {
    _continuations.Enqueue(new Continuation(callback, state));
  }

  public override void Send(SendOrPostCallback callback, object? state)
  {
    if (Thread.CurrentThread == mainThread)
    {
      callback(state);
    }
    else
    {
      _continuations.Enqueue(new Continuation(callback, state));
    }
  }

  private readonly record struct Continuation(SendOrPostCallback Callback, object? State)
  {
    public void Execute() => Callback(State);
  }
}
