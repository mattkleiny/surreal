using Surreal.Assets;
using Surreal.Audio;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.IO;
using Surreal.Services;
using Surreal.Timing;

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
  public HeadlessAudioServer AudioServer { get; } = new();
  public HeadlessGraphicsServer GraphicsServer { get; } = new();
  public HeadlessInputServer InputServer { get; } = new();

  public event Action<int, int>? Resized;

  public int Width => 1920;
  public int Height => 1080;
  public bool IsVisible => true;
  public bool IsFocused => true;
  public bool IsClosing => false;

  public HeadlessKeyboardDevice Keyboard => InputServer.Keyboard;
  public HeadlessMouseDevice Mouse => InputServer.Mouse;

  public void RegisterServices(IServiceRegistry services)
  {
    services.AddService<IPlatformHost>(this);
    services.AddService<IHeadlessPlatformHost>(this);
    services.AddService<IAudioServer>(AudioServer);
    services.AddService<IGraphicsServer>(GraphicsServer);
    services.AddService<IInputServer>(InputServer);
    services.AddService<IKeyboardDevice>(InputServer.Keyboard);
    services.AddService<IMouseDevice>(InputServer.Mouse);
  }

  public void RegisterAssetLoaders(IAssetManager manager)
  {
    // no-op
  }

  public void RegisterFileSystems(IFileSystemRegistry registry)
  {
    // no-op
  }

  public void BeginFrame(TimeDelta deltaTime)
  {
  }

  public void EndFrame(TimeDelta deltaTime)
  {
    // no-op
  }

  public void Dispose()
  {
    // no-op
  }
}
