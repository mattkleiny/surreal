using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Surreal.Editor.Projects;

namespace Surreal.Editor;

/// <summary>
/// Root class for the editor application.
/// </summary>
public partial class App : Application
{
  /// <summary>
  /// The current project.
  /// </summary>
  public Project CurrentProject { get; private set; } = new(
    Path.Combine(Environment.CurrentDirectory, "Assets"),
    Path.Combine(Environment.CurrentDirectory, "Target")
  );

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
        DataContext = CurrentProject
      };
    }

    base.OnFrameworkInitializationCompleted();
  }
}
