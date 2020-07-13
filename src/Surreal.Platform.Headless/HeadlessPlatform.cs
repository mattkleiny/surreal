namespace Surreal.Platform {
  public sealed class HeadlessPlatform : IPlatform {
    public IPlatformHost BuildHost() => new HeadlessPlatformHost();
  }
}