namespace Surreal;

/// <summary>Configuration for the <see cref="ConsolePlatform"/>.</summary>
public sealed record ConsoleConfiguration
{
}

/// <summary>A <see cref="IPlatform"/> for console-only desktop environments.</summary>
public sealed class ConsolePlatform : IPlatform
{
  public ConsoleConfiguration Configuration { get; } = new();

  public IPlatformHost BuildHost()
  {
    return new ConsolePlatformHost(Configuration);
  }
}
