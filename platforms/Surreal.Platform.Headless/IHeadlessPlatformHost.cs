namespace Surreal.Platform
{
  public interface IHeadlessPlatformHost : IPlatformHost
  {
    IHeadlessKeyboardDevice Keyboard { get; }
    IHeadlessMouseDevice    Mouse    { get; }
  }
}