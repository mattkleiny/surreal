using Surreal.Timing;

namespace Surreal;

/// <summary>Configuration for the <see cref="ConsolePlatform"/>.</summary>
public sealed record ConsoleConfiguration
{
  public string Title    { get; set; } = "Surreal";
  public int    Width    { get; set; } = 280;
  public int    Height   { get; set; } = 120;
  public string Font     { get; set; } = "Courier New";
  public short  FontSize { get; set; } = 7;

  /// <summary>Periodically add the FPS to the console title?</summary>
  public bool ShowFpsInTitle { get; set; } = false;

  /// <summary>A time limit on frames per second.</summary>
  public TimeSpan TargetDeltaTime { get; set; } = 16.Milliseconds();
}

/// <summary>A <see cref="IPlatform"/> for console-only desktop environments.</summary>
public sealed class ConsolePlatform : IPlatform
{
  public ConsoleConfiguration Configuration { get; } = new();

  public IPlatformHost BuildHost()
  {
    if (!OperatingSystem.IsWindows())
    {
      throw new InvalidOperationException("The console platform is only supported on Windows, currently");
    }

    return new ConsolePlatformHost(Configuration);
  }
}
