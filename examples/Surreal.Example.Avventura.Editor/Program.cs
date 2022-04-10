using Avalonia;
using Avalonia.ReactiveUI;

namespace Avventura;

public static class Program
{
  [STAThread]
  public static void Main(string[] args)
  {
    AppBuilder
      .Configure<Application>()
      .UsePlatformDetect()
      .LogToTrace()
      .UseReactiveUI()
      .StartWithClassicDesktopLifetime(args);
  }
}
