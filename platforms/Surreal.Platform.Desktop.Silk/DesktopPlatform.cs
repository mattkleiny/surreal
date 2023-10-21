﻿using Surreal.IO;

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
  public bool WaitForFirstFrame { get; set; } = true;
  public bool ShowFpsInTitle { get; set; } = true;
  public bool RunInBackground { get; set; }
  public VirtualPath? IconPath { get; set; } = "resx://Surreal/Assets/Embedded/surreal.png";
}

/// <summary>
/// A <see cref="IPlatformHostFactory" /> for desktop environments.
/// </summary>
public sealed class DesktopPlatform : IPlatformHostFactory
{
  public DesktopConfiguration Configuration { get; } = new();

  public IPlatformHost BuildHost(IGameHost host)
  {
    return new DesktopPlatformHost(Configuration, host);
  }
}
