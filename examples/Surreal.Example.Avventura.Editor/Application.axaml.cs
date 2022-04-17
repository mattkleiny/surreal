using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avventura.Windows;
using Surreal.Workloads;

namespace Avventura
{
  public partial class Application : EditorApplication<AvventuraGame>
  {
    public override IEnumerable<EditorWorkload> Workloads
    {
      get { yield return new GameViewWorkload(); }
    }

    public override void Initialize()
    {
      AvaloniaXamlLoader.Load(this);

      base.Initialize();
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
