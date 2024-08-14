using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using Silk.NET.OpenGL;
using Surreal.Audio;
using Surreal.Editing.Common;
using Surreal.Graphics;
using Surreal.Hosting;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Services;
using Surreal.Timing;

namespace Surreal.Editing.Components;

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

  /// <summary>
  /// Invoked when the control is rendered.
  /// </summary>
  public event RenderCallback? Rendering;

  /// <summary>
  /// The Silk.NET OpenGL interface.
  /// </summary>
  public GL? OpenGL { get; private set; }

  protected override void OnOpenGlInit(GlInterface gl)
  {
    OpenGL = GL.GetApi(gl.GetProcAddress);
  }

  protected override void OnOpenGlRender(GlInterface gl, int frameBufferId)
  {
    var frameBuffer = GraphicsHandle.FromInt(frameBufferId);

    Rendering?.Invoke(gl, frameBuffer);
  }
}

/// <summary>
/// A view model for the <see cref="GameViewport"/>.
/// </summary>
internal sealed class GameViewportViewModel : EditorViewModel
{
  private readonly HostingContext _context;
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
  /// The <see cref="HostingContext"/> for the main window.
  /// </summary>
  private sealed class EditorHostingContext(GameViewportViewModel owner) : HostingContext
  {
    /// <inheritdoc/>
    public override IPlatformHost PlatformHost { get; } = new EditorPlatformHost(owner);

    public override void OnStarted()
    {
      Dispatcher.UIThread.Post(() => owner.IsRunning = true);
    }

    public override void OnStopped()
    {
      Dispatcher.UIThread.Post(() => owner.IsRunning = false);
    }
  }

  /// <summary>
  /// A <see cref="IPlatformHost"/> for the <see cref="EditorHostingContext"/>.
  /// </summary>
  private sealed class EditorPlatformHost : IPlatformHost
  {
    private readonly GameViewportViewModel _owner;

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

    public event Action<int, int>? Resized;

    public int Width => (int)_owner.Viewport.Width;
    public int Height => (int)_owner.Viewport.Height;
    public bool IsVisible => _owner.Viewport.IsVisible;
    public bool IsFocused => _owner.Viewport.IsFocused;
    public bool IsClosing => false;

    public void RegisterServices(IServiceRegistry services)
    {
      services.AddService(IAudioBackend.Null);
      services.AddService<IGraphicsBackend>(new SilkGraphicsBackend(_owner.Viewport.Display.OpenGL!));
      services.AddService(IKeyboardDevice.Null);
      services.AddService(IMouseDevice.Null);
    }

    public void BeginFrame(DeltaTime deltaTime)
    {
    }

    public void EndFrame(DeltaTime deltaTime)
    {
    }

    public void Dispose()
    {
    }

    private void OnDisplayRendering(GlInterface gl, GraphicsHandle frameBuffer)
    {
      gl.BindFramebuffer(GlConsts.GL_READ_FRAMEBUFFER, 0);
      gl.BindFramebuffer(GlConsts.GL_DRAW_FRAMEBUFFER, frameBuffer);
    }
  }
}
