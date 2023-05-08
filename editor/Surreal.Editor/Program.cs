using Avalonia;

namespace Surreal.Editor;

public static class Program
{
  [STAThread]
  public static void Main(string[] args)
  {
    BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
  }

  [UsedImplicitly]
  public static AppBuilder BuildAvaloniaApp() => AppBuilder
    .Configure<App>()
    .UsePlatformDetect()
    .LogToTrace();
}
