namespace Surreal.Platform;

/// <summary>Configuration for the <see cref="DesktopPlatform"/>.</summary>
public sealed record DesktopConfiguration
{
  public string Title          { get; set; } = "Surreal";
  public int    Width          { get; set; } = 1920;
  public int    Height         { get; set; } = 1080;
  public bool   IsVisible      { get; set; } = true;
  public bool   IsResizable    { get; set; } = true;
  public bool   IsVsyncEnabled { get; set; } = false;
  public bool   ShowFPSInTitle { get; set; } = false;
}