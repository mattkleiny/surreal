using Avalonia.Controls;
using Avalonia.Threading;
using Surreal.Diagnostics.Hosting;
using Surreal.Editing.Projects;
using Surreal.Editing.ViewModels;

namespace Surreal.Editing;

/// <summary>
/// The main window for the editor.
/// </summary>
internal partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();

    DataContext = new MainWindowViewModel();
  }
}

/// <summary>
/// A view model for the main window.
/// </summary>
internal sealed class MainWindowViewModel : ViewModel
{
  private bool _isRunning;
  private HostingContext? _hostingContext;

  public MainWindowViewModel()
  {
    StartGame = new Command(OnStartGame, () => !IsRunning);
    StopGame = new Command(OnStopGame, () => IsRunning);
  }

  public Command StartGame { get; }
  public Command StopGame { get; }

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

  /// <summary>
  /// The current <see cref="EditorConfiguration"/>.
  /// </summary>
  public EditorConfiguration Configuration => EditorApplication.Current!.Configuration;

  /// <summary>
  /// The current <see cref="Project"/>.
  /// </summary>
  public EditorProject? Project => EditorApplication.Current!.Project;

  private void OnStartGame()
  {
    _hostingContext = new HostingContextImpl(this);

    EditorApplication.Current?.Project?.ProjectHost?.StartAsync(_hostingContext);
  }

  private void OnStopGame()
  {
    _hostingContext?.NotifyCancelled();
  }

  /// <summary>
  /// The <see cref="HostingContext"/> for the main window.
  /// </summary>
  private sealed class HostingContextImpl(MainWindowViewModel window) : HostingContext
  {
    public override IServiceProvider Services => EditorApplication.Current?.Services ?? throw new InvalidOperationException("The editor application stopped before the game");

    public override void NotifyStarted()
    {
      base.NotifyStarted();

      Dispatcher.UIThread.Post(() => window.IsRunning = true);
    }

    public override void NotifyStopped()
    {
      base.NotifyStopped();

      Dispatcher.UIThread.Post(() => window.IsRunning = false);
    }
  }
}
