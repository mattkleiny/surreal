namespace Surreal;

/// <summary>
/// A <see cref="IPlatformHostFactory" /> for headless environments.
/// </summary>
public sealed class HeadlessPlatform : IPlatformHostFactory
{
  public IPlatformHost BuildHost(IGameHost host)
  {
    return new HeadlessPlatformHost();
  }
}
