using Surreal.IO;
using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// Configuration for the <see cref="DesktopPlatform" />.
/// </summary>
public sealed record DesktopConfiguration
{
  public string Title { get; set; } = "Surreal";
  public int Width { get; set; } = 1920;
  public int Height { get; set; } = 1080;
  public bool IsResizable { get; set; } = true;
  public bool IsVsyncEnabled { get; set; } = true;
  public bool IsEventDriven { get; set; }
  public bool IsTransparent { get; set; }
  public bool ShowFpsInTitle { get; set; } = true;
  public VirtualPath? IconPath { get; set; } = "resx://Surreal/Assets/Embedded/surreal.png";
}

/// <summary>
/// A <see cref="IPlatform" /> for desktop environments.
/// </summary>
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("macos")]
[SupportedOSPlatform("linux")]
public sealed class DesktopPlatform : IPlatform
{
  public DesktopConfiguration Configuration { get; } = new();

  public IPlatformHost BuildHost(IServiceRegistry services)
  {
    return new DesktopPlatformHost(Configuration, services);
  }
}
