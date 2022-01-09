namespace Surreal;

/// <summary>Configuration for the <see cref="MobilePlatform"/>.</summary>
public sealed record MobileConfiguration
{
  public int  Width          { get; set; } = 1920;
  public int  Height         { get; set; } = 1080;
  public bool IsVsyncEnabled { get; set; }
}

/// <summary>A <see cref="IPlatform"/> for mobile devices.</summary>
public sealed class MobilePlatform : IPlatform
{
  public MobileConfiguration Configuration { get; } = new();

  public IPlatformHost BuildHost()
  {
    return new MobilePlatformHost(Configuration);
  }
}
