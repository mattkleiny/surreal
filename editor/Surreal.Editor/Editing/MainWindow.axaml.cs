using Avalonia.Controls;
using Avalonia.Threading;
using Surreal.Audio;
using Surreal.Editing.ViewModels;
using Surreal.Graphics;
using Surreal.Hosting;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal.Editing;

/// <summary>
/// The main window for the editor.
/// </summary>
internal partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();

    DataContext = new MainWindowViewModel(this);
  }
}

/// <summary>
/// A view model for the main window.
/// </summary>
internal sealed class MainWindowViewModel : EditorViewModel
{
  private readonly MainWindow _mainWindow;
  private readonly HostingContext _hostingContext;
  private bool _isRunning;

  public MainWindowViewModel(MainWindow mainWindow)
  {
    _mainWindow = mainWindow;
    _hostingContext = new EditorHostingContext(this);

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
    Project?.Host?.StartAsync(_hostingContext);
  }

  private void OnStopGame()
  {
    _hostingContext.NotifyCancelled();
  }

  /// <summary>
  /// The <see cref="HostingContext"/> for the main window.
  /// </summary>
  private sealed class EditorHostingContext(MainWindowViewModel viewModel) : HostingContext
  {
    /// <inheritdoc/>
    public override IPlatformHost PlatformHost { get; } = new EditorPlatformHost(viewModel);

    /// <inheritdoc/>
    public override void NotifyStarted()
    {
      base.NotifyStarted();

      Dispatcher.UIThread.Post(() => viewModel.IsRunning = true);
    }

    /// <inheritdoc/>
    public override void NotifyStopped()
    {
      base.NotifyStopped();

      Dispatcher.UIThread.Post(() => viewModel.IsRunning = false);
    }

    /// <summary>
    /// A <see cref="IPlatformHost"/> for the <see cref="EditorHostingContext"/>.
    /// </summary>
    private sealed class EditorPlatformHost : IPlatformHost
    {
      private readonly MainWindowViewModel _viewModel;

      public EditorPlatformHost(MainWindowViewModel viewModel)
      {
        _viewModel = viewModel;

        // forward resize events
        _viewModel._mainWindow.Resized += (_, e) =>
        {
          Resized?.Invoke((int)e.ClientSize.Width, (int)e.ClientSize.Height);
        };
      }

      public event Action<int, int>? Resized;

      public int Width => (int)_viewModel._mainWindow.Width;
      public int Height => (int)_viewModel._mainWindow.Height;
      public bool IsVisible => _viewModel._mainWindow.IsVisible;
      public bool IsFocused => true; // TODO: implement me properly sometime
      public bool IsClosing => !_viewModel.IsRunning;

      public void RegisterServices(IServiceRegistry services)
      {
        services.AddService(IAudioBackend.Headless);
        services.AddService(IGraphicsBackend.Headless);
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
