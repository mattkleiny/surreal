using Surreal.Platform.Internal;

namespace Surreal.Platform
{
  public sealed class DesktopPlatform : IPlatform
  {
    public DesktopConfiguration Configuration { get; } = new();

    public IPlatformHost BuildHost()
    {
      var window = Configuration.CustomWindow ?? new OpenTKWindow(Configuration);

      return new DesktopPlatformHost(window, Configuration);
    }
  }
}