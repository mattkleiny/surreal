using Surreal.Assets;
using Surreal.Audio;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Internal.Audio;
using Surreal.Internal.Graphics;
using Surreal.Internal.Input;
using Surreal.IO;
using Surreal.Timing;

namespace Surreal;

/// <summary>A specialization of <see cref="IPlatformHost"/> for headless environments.</summary>
public interface IHeadlessPlatformHost : IPlatformHost
{
  IHeadlessKeyboardDevice Keyboard { get; }
  IHeadlessMouseDevice    Mouse    { get; }
}

internal sealed class HeadlessPlatformHost : IHeadlessPlatformHost
{
  public event Action<int, int> Resized = null!;

  public HeadlessAudioServer    AudioServer    { get; } = new();
  public HeadlessGraphicsServer GraphicsServer { get; } = new();
  public HeadlessInputServer    InputServer    { get; } = new();

  public int  Width     => 1920;
  public int  Height    => 1080;
  public bool IsVisible => true;
  public bool IsFocused => true;
  public bool IsClosing => false;

  public IHeadlessKeyboardDevice Keyboard => InputServer.Keyboard;
  public IHeadlessMouseDevice    Mouse    => InputServer.Mouse;

  public void RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton<IPlatformHost>(this);
    services.AddSingleton<IHeadlessPlatformHost>(this);
    services.AddSingleton<IAudioServer>(AudioServer);
    services.AddSingleton<IGraphicsServer>(GraphicsServer);
    services.AddSingleton<IInputServer>(InputServer);
    services.AddSingleton<IKeyboardDevice>(InputServer.Keyboard);
    services.AddSingleton<IMouseDevice>(InputServer.Mouse);
    services.AddSingleton<IHeadlessKeyboardDevice>(InputServer.Keyboard);
    services.AddSingleton<IHeadlessMouseDevice>(InputServer.Mouse);
  }

  public void RegisterAssetLoaders(IAssetManager manager)
  {
    // no-op
  }

  public void RegisterFileSystems(IFileSystemRegistry registry)
  {
    // no-op
  }

  public void BeginFrame(DeltaTime deltaTime)
  {
    InputServer.Update();
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
