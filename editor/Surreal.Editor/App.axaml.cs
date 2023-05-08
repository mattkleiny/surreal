using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Surreal.Editor;

/// <summary>
/// Root class for the editor application.
/// </summary>
public partial class App : Application
{
  /// <summary>
  /// The current project.
  /// </summary>
  public Project CurrentProject { get; } = new(Environment.CurrentDirectory);

  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      desktop.MainWindow = new MainWindow
      {
        Project = CurrentProject
      };
    }

    base.OnFrameworkInitializationCompleted();
  }
}
