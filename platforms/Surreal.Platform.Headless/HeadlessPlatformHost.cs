using Surreal.Audio;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.IO;
using Surreal.Resources;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// A specialization of <see cref="IPlatformHost" /> for headless environments.
/// </summary>
public interface IHeadlessPlatformHost : IPlatformHost
{
  HeadlessKeyboardDevice Keyboard { get; }
  HeadlessMouseDevice Mouse { get; }
}

internal sealed class HeadlessPlatformHost : IHeadlessPlatformHost
{
  public HeadlessAudioBackend AudioBackend { get; } = new();
  public HeadlessGraphicsBackend GraphicsBackend { get; } = new();
  public HeadlessInputBackend InputBackend { get; } = new();

  public event Action<int, int>? Resized;

  public int Width => 1920;
  public int Height => 1080;
  public bool IsVisible => true;
  public bool IsFocused => true;
  public bool IsClosing => false;

  public HeadlessKeyboardDevice Keyboard => InputBackend.Keyboard;
  public HeadlessMouseDevice Mouse => InputBackend.Mouse;

  public void RegisterServices(IServiceRegistry services)
  {
  }

  public void RegisterAssetLoaders(IResourceManager manager)
  {
    // no-op
  }

  public void RegisterFileSystems(IFileSystemRegistry registry)
  {
    // no-op
  }

  public void BeginFrame(DeltaTime deltaTime)
  {
    // no-op
  }

  public void EndFrame(DeltaTime deltaTime)
  {
    // no-op
  }

  public void Dispose()
  {
    // no-op
  }
}
