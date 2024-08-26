using Avalonia;
using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using Silk.NET.OpenGL;
using Surreal.Audio;
using Surreal.Editing.Common;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Services;
using Surreal.Timing;

namespace Surreal.Editing.Components;

#pragma warning disable CS0618 // Type or member is obsolete

/// <summary>
/// Hosts the game inside a viewport of the editor.
/// </summary>
internal partial class GameViewport : UserControl
{
  public GameViewport()
  {
    InitializeComponent();

    DataContext = new GameViewportViewModel(this);
  }
}

/// <summary>
/// A control that hosts the game viewport display.
/// </summary>
internal sealed class GameViewportDisplay : OpenGlControlBase
{
  public delegate void RenderCallback(GlInterface gl, GraphicsHandle frameBuffer);

  // the underlying graphics service from Avalonia.
  private readonly IPlatformGraphics? _graphics = (IPlatformGraphics?)GetService(GetCurrentLocator(), typeof(IPlatformGraphics));

  /// <summary>
  /// Invoked when the control is rendered.
  /// </summary>
  public event RenderCallback? Rendering;

  /// <summary>
  /// The Silk.NET OpenGL interface.
  /// </summary>
  public GL? OpenGL { get; private set; }

  /// <summary>
  /// Makes the control's OpenGL context current.
  /// </summary>
  public IDisposable MakeCurrent()
  {
    if (_graphics == null)
    {
      throw new InvalidOperationException("The graphics service is not available.");
    }

    return _graphics.GetSharedContext().EnsureCurrent();
  }

  protected override void OnOpenGlInit(GlInterface gl)
  {
    OpenGL = GL.GetApi(gl.GetProcAddress);
  }

  protected override void OnOpenGlRender(GlInterface gl, int frameBufferId)
  {
    var frameBuffer = GraphicsHandle.FromInt(frameBufferId);

    Rendering?.Invoke(gl, frameBuffer);
  }

  [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetService")]
  private static extern object? GetService(IAvaloniaDependencyResolver resolver, Type serviceType);

  [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "get_Current")]
  private static extern IAvaloniaDependencyResolver GetCurrentLocator(AvaloniaLocator locator = null!);
}

/// <summary>
/// A view model for the <see cref="GameViewport"/>.
/// </summary>
internal sealed class GameViewportViewModel : EditorViewModel
{
  private readonly GameContext _context;
  private bool _isRunning;

  public GameViewportViewModel(GameViewport viewport)
  {
    Viewport = viewport;

    _context = new EditorHostingContext(this);

    StartGame = new EditorCommand(OnStartGame, () => !IsRunning);
    StopGame = new EditorCommand(OnStopGame, () => IsRunning);
  }

  public EditorCommand StartGame { get; }
  public EditorCommand StopGame { get; }

  /// <summary>
  /// The <see cref="GameViewport"/> that this view model is for.
  /// </summary>
  public GameViewport Viewport { get; }

  /// <summary>
  /// True if the game is currently running.
  /// </summary>
  public bool IsRunning
  {
    get => _isRunning;
    set
    {
      if (SetField(ref _isRunning, value))
      {
        StartGame.NotifyCanExecuteChanged();
        StopGame.NotifyCanExecuteChanged();
      }
    }
  }

  private void OnStartGame()
  {
    Project?.Host?.StartAsync(_context);
  }

  private void OnStopGame()
  {
    _context.NotifyCancelled();
  }

  /// <summary>
  /// The <see cref="GameContext"/> for the main window.
  /// </summary>
  private sealed class EditorHostingContext(GameViewportViewModel owner) : GameContext
  {
    /// <inheritdoc/>
    public override IPlatformHost PlatformHost { get; } = new EditorPlatformHost(owner);

    /// <inheritdoc/>
    public override Task InvokeOnMainThread(Func<Task> action)
    {
      return Dispatcher.UIThread.InvokeAsync(WithContextGuard(action), DispatcherPriority.Background);
    }

    public IDisposable? ContextGuard { get; private set; }

    /// <summary>
    /// Runs the given action with the OpenGL context current.
    /// </summary>
    private Func<Task> WithContextGuard(Func<Task> action) => async () =>
    {
      // TODO: clean this up
      ContextGuard = owner.Viewport.Display.MakeCurrent();

      await action();
    };
  }

  /// <summary>
  /// A <see cref="IPlatformHost"/> for the <see cref="EditorHostingContext"/>.
  /// </summary>
  private sealed class EditorPlatformHost : IPlatformHost
  {
    private readonly DeltaTimeClock _clock = new();
    private readonly GameViewportViewModel _owner;

    private DeltaTime _deltaTime;

    public EditorPlatformHost(GameViewportViewModel owner)
    {
      _owner = owner;

      _owner.Viewport.SizeChanged += (_, e) =>
      {
        // forward resize events
        Resized?.Invoke((int)e.NewSize.Width, (int)e.NewSize.Height);
      };

      _owner.Viewport.Display.Rendering += OnDisplayRendering;
    }

    public event Action<DeltaTime>? Update;
    public event Action<DeltaTime>? Render;

    public event Action<int, int>? Resized;

    public int Width => (int)_owner.Viewport.Width;
    public int Height => (int)_owner.Viewport.Height;
    public bool IsVisible => true;
    public bool IsFocused => true;
    public bool IsClosing => !_owner.IsRunning;

    public void RegisterServices(IServiceRegistry services)
    {
      services.AddService(IAudioBackend.Null);
      services.AddService<IGraphicsBackend>(new EditorGraphicsBackend(_owner.Viewport.Display.OpenGL!));
      services.AddService(IInputBackend.Null);
      services.AddService(IKeyboardDevice.Null);
      services.AddService(IMouseDevice.Null);
    }

    public Task RunAsync()
    {
      var frame = new DispatcherFrame();
      var context = (EditorHostingContext)_owner._context;

      context.ContextGuard?.Dispose();

      frame.Dispatcher.Post(async () =>
      {
        _owner.IsRunning = true;
        _owner.Viewport.Display.RequestNextFrameRendering();

        while (frame.Continue && _owner.IsRunning)
        {
          _deltaTime = _clock.Tick();
          Update?.Invoke(_deltaTime);

          if (_deltaTime < _clock.MinDeltaTime)
            await Task.Delay(_clock.TargetDeltaTime - _deltaTime);
          else
            await Task.Yield();
        }

        frame.Continue = false;

        _owner.IsRunning = false;
        _owner.Viewport.Display.RequestNextFrameRendering();
      });

      Dispatcher.UIThread.PushFrame(frame);

      return Task.CompletedTask;
    }

    public void Close()
    {
      Dispatcher.UIThread.Post(() => _owner.IsRunning = false);
    }

    public void Dispose()
    {
    }

    private void OnDisplayRendering(GlInterface gl, GraphicsHandle frameBuffer)
    {
      gl.BindFramebuffer(GlConsts.GL_READ_FRAMEBUFFER, 0);
      gl.BindFramebuffer(GlConsts.GL_DRAW_FRAMEBUFFER, frameBuffer);

      gl.ClearColor(0, 0, 0, 1);
      gl.Clear(GlConsts.GL_COLOR_BUFFER_BIT | GlConsts.GL_DEPTH_BUFFER_BIT);

      // continually re-render
      if (_owner.IsRunning)
      {
        Render?.Invoke(_deltaTime);

        _owner.Viewport.Display.RequestNextFrameRendering();
      }
    }
  }

  /// <summary>
  /// A <see cref="IGraphicsBackend"/> implementation for the editor.
  /// </summary>
  private sealed class EditorGraphicsBackend(GL gl) : IGraphicsBackend
  {
    public IGraphicsDevice CreateDevice(GraphicsMode mode)
    {
      return new SilkGraphicsDeviceOpenGL(gl);
    }
  }
}
