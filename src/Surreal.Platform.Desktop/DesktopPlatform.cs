namespace Surreal.Platform
{
  public sealed class DesktopPlatform : IPlatform
  {
    public DesktopConfiguration Configuration { get; } = new();

    public IPlatformHost BuildHost()
    {
      return new DesktopPlatformHost(Configuration);
    }
  }
}
