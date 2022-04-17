using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avventura.Windows;

namespace Avventura
{
  public partial class Application : EditorApplication<AvventuraGame>
  {
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
          DataContext = Editor.ViewModel,
        };
      }

      base.OnFrameworkInitializationCompleted();
    }
  }
}
