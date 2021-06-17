namespace Surreal.Platform {
  public interface IDesktopPlatformHost : IPlatformHost {
    IDesktopWindow Window { get; }
  }
}