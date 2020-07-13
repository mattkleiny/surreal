namespace Surreal.Platform {
  public interface IDesktopPlatformHost : IPlatformHost {
    string   Title          { get; set; }
    bool     IsVsyncEnabled { get; set; }
    new int  Width          { get; set; }
    new int  Height         { get; set; }
    new bool IsVisible      { get; set; }
  }
}