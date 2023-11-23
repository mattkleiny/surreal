using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Surreal.Editing.Projects;
using Surreal.Services;

namespace Surreal.Editing;

/// <summary>
/// Root class for the editor application.
/// </summary>
internal sealed class EditorApplication : Application, IDisposable
{
  /// <summary>
  /// The current <see cref="EditorApplication"/> instance.
  /// </summary>
  public new static EditorApplication? Current => (EditorApplication?)Application.Current;

  private readonly ServiceRegistry _services;
  private EditorProject? _project;

  [UsedImplicitly]
  public EditorApplication()
    : this(new EditorConfiguration())
  {
  }

  public EditorApplication(EditorConfiguration configuration)
  {
    _services = new ServiceRegistry();

    foreach (var module in configuration.Modules)
    {
      _services.AddModule(module);
    }

    Configuration = configuration;
    Project = configuration.DefaultProject;
  }

  /// <summary>
  /// The <see cref="EditorConfiguration"/>.
  /// </summary>
  public EditorConfiguration Configuration { get; }

  /// <summary>
  /// The currently active <see cref="EditorProject"/>.
  /// </summary>
  public EditorProject? Project
  {
    get => _project;
    set
    {
      if (_project != value)
      {
        Context?.Dispose();
        _project = value;

        if (value != null)
        {
          Context = new EditorProjectContext(Configuration, value, _services);
        }
      }
    }
  }

  /// <summary>
  /// The <see cref="EditorProjectContext"/> for the active <see cref="Project"/>.
  /// </summary>
  public EditorProjectContext? Context { get; private set; }

  /// <summary>
  /// The root <see cref="IServiceProvider"/> for the editor.
  /// </summary>
  public IServiceProvider Services => _services;

  /// <summary>
  /// The top-level <see cref="IEventBus"/> for the editor.
  /// </summary>
  public IEventBus Events { get; } = new EventBus();

  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      desktop.MainWindow = new MainWindow();
    }

    base.OnFrameworkInitializationCompleted();
  }

  public void Dispose()
  {
    _services.Dispose();
  }
}
