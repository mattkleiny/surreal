using Surreal.Graphics.Images;
using Surreal.Internal;

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

  /// <summary>The desired <see cref="IDesktopBackend"/> for the application.</summary>
  public IDesktopBackend Backend { get; set; } = DesktopBackends.OpenTK;
}

/// <summary>The backend platform for <see cref="DesktopPlatform"/>s.</summary>
public interface IDesktopBackend
{
}

/// <summary>Commonly used <see cref="IDesktopBackend"/>s.</summary>
public static class DesktopBackends
{
  public static IDesktopBackend OpenTK { get; } = new OpenTKDesktopBackend();
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
