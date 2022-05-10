using Avalonia;

namespace Surreal;

public static class Program
{
  [STAThread]
  public static int Main(string[] args)
  {
    return AppBuilder.Configure<App>()
      .UsePlatformDetect()
      .LogToTrace()
      .StartWithClassicDesktopLifetime(args);
  }
}
