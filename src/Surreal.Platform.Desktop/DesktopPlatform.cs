using Surreal.Platform.Internal;

namespace Surreal.Platform {
  public sealed class DesktopPlatform : IPlatform {
    public DesktopConfiguration Configuration { get; } = new DesktopConfiguration();

    public IPlatformHost BuildHost() {
      var window = new OpenTKWindow(Configuration);

      return new DesktopPlatformHost(window, Configuration);
    }
  }
}