namespace Surreal.Platform;

/// <summary>A specialization of <see cref="IPlatformHost"/> for headless environments.</summary>
public interface IHeadlessPlatformHost : IPlatformHost
{
  IHeadlessKeyboardDevice Keyboard { get; }
  IHeadlessMouseDevice    Mouse    { get; }
}