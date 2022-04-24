using Surreal.Graphics.Images;

namespace Surreal;

/// <summary>Configuration for the <see cref="DesktopPlatform"/>.</summary>
public sealed record DesktopConfiguration
{
  public string Title          { get; set; } = "Surreal";
  public int    Width          { get; set; } = 1920;
  public int    Height         { get; set; } = 1080;
  public bool   IsResizable    { get; set; } = true;
  public bool   IsVsyncEnabled { get; set; }
  public bool   ShowFpsInTitle { get; set; }
  public Image? Icon           { get; set; }
}

/// <summary>A <see cref="IPlatform"/> for desktop environments.</summary>
public sealed class DesktopPlatform : IPlatform
{
  public DesktopConfiguration Configuration { get; } = new();

  public IPlatformHost BuildHost()
  {
    return new DesktopPlatformHost(Configuration);
  }
}
