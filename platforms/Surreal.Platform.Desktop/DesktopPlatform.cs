namespace Surreal;

/// <summary>A <see cref="IPlatform"/> for desktop environments.</summary>
public sealed class DesktopPlatform : IPlatform
{
  public DesktopConfiguration Configuration { get; } = new();

  public IPlatformHost BuildHost()
  {
    return new DesktopPlatformHost(Configuration);
  }
}