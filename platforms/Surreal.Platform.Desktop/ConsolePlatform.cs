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
}

/// <summary>A <see cref="IPlatform"/> for console-only desktop environments.</summary>
public sealed class ConsolePlatform : IPlatform
{
  public ConsoleConfiguration Configuration { get; } = new();

  public IPlatformHost BuildHost()
  {
    if (OperatingSystem.IsWindows())
    {
      return new ConsolePlatformHost(Configuration);
    }

    throw new InvalidOperationException("The console platform is not supported on this operating system");
  }
}
