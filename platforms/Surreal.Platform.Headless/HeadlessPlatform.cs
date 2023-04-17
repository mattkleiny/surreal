namespace Surreal;

/// <summary>
/// A <see cref="IPlatform" /> for headless environments.
/// </summary>
public sealed class HeadlessPlatform : IPlatform
{
  public IPlatformHost BuildHost()
  {
    return new HeadlessPlatformHost();
  }
}
