using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using Surreal.Audio;
using Surreal.Editing.Backends;
using Surreal.Editing.Common;
using Surreal.Graphics;
using Surreal.Hosting;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal.Editing.Components;

/// <summary>
/// Hosts the game inside a viewport of the editor.
/// </summary>
internal partial class GameViewport : UserControl
{
  public GameViewport()
  {
    InitializeComponent();

    DataContext = new GameViewportViewModel(this, Host);
  }
}

/// <summary>
/// An <see cref="OpenGlControlBase"/> that hosts the game viewport.
/// </summary>
internal sealed class GameViewportHost : OpenGlControlBase
{
  /// <summary>
  /// The <see cref="GlInterface"/> for the control.
  /// </summary>
  public GlInterface? OpenGl { get; private set; }

  protected override void OnOpenGlInit(GlInterface gl)
  {
    OpenGl = gl;
  }

  protected override void OnOpenGlRender(GlInterface gl, int fb)
  {
  }
}

/// <summary>
/// A view model for the <see cref="GameViewport"/>.
/// </summary>
internal sealed class GameViewportViewModel : EditorViewModel
{
  private readonly GameViewport _viewport;
  private readonly GameViewportHost _viewportHost;
  private readonly HostingContext _context;
  private bool _isRunning;

  public GameViewportViewModel(GameViewport viewport, GameViewportHost viewportHost)
  {
    _viewport = viewport;
    _viewportHost = viewportHost;

    _context = new EditorHostingContext(this);

    StartGame = new EditorCommand(OnStartGame, () => !IsRunning);
    StopGame = new EditorCommand(OnStopGame, () => IsRunning);
  }

  public EditorCommand StartGame { get; }
  public EditorCommand StopGame { get; }

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

    /// <summary>
    /// A <see cref="IPlatformHost"/> for the <see cref="EditorHostingContext"/>.
    /// </summary>
    private sealed class EditorPlatformHost : IPlatformHost
    {
      private readonly GameViewportViewModel _owner;

      public EditorPlatformHost(GameViewportViewModel owner)
      {
        _owner = owner;

        // TODO: forward resize events
        // _viewModel._viewport.RResized += (_, e) => { Resized?.Invoke((int)e.ClientSize.Width, (int)e.ClientSize.Height); };
      }

      public event Action<int, int>? Resized;

      public int Width => (int)_owner._viewport.Width;
      public int Height => (int)_owner._viewport.Height;
      public bool IsVisible => _owner._viewport.IsVisible;
      public bool IsFocused => true; // TODO: implement me
      public bool IsClosing => !_owner.IsRunning;

      public void RegisterServices(IServiceRegistry services)
      {
        services.AddService(IAudioBackend.Headless);
        services.AddService<IGraphicsBackend>(new EditorGraphicsBackend(_owner._viewportHost.OpenGl!));
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
    }
  }
}
