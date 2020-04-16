using System;
using Surreal.Audio.SPI;
using Surreal.Compute.SPI;
using Surreal.Graphics.SPI;
using Surreal.Input;
using Surreal.IO;
using Surreal.Platform.Internal.Audio;
using Surreal.Platform.Internal.Compute;
using Surreal.Platform.Internal.Graphics;
using Surreal.Platform.Internal.Input;
using Surreal.Timing;

namespace Surreal.Platform
{
  internal sealed class HeadlessPlatformHost : IHeadlessPlatformHost, IServiceProvider
  {
    public HeadlessAudioBackend    AudioBackend    { get; } = new HeadlessAudioBackend();
    public HeadlessComputeBackend  ComputeBackend  { get; } = new HeadlessComputeBackend();
    public HeadlessGraphicsBackend GraphicsBackend { get; } = new HeadlessGraphicsBackend();
    public HeadlessInputManager    InputManager    { get; } = new HeadlessInputManager();
    public LocalFileSystem         FileSystem      { get; } = new LocalFileSystem();

    public event Action<int, int> Resized;

    public int  Width     { get; } = 1920;
    public int  Height    { get; } = 1080;
    public bool IsVisible { get; } = true;
    public bool IsFocused { get; } = true;
    public bool IsClosing { get; } = false;

    public IServiceProvider Services => this;

    public IHeadlessKeyboardDevice Keyboard => InputManager.Keyboard;
    public IHeadlessMouseDevice    Mouse    => InputManager.Mouse;

    object? IServiceProvider.GetService(Type serviceType)
    {
      if (serviceType == typeof(IAudioBackend)) return AudioBackend;
      if (serviceType == typeof(IComputeBackend)) return ComputeBackend;
      if (serviceType == typeof(IGraphicsBackend)) return GraphicsBackend;
      if (serviceType == typeof(IInputManager)) return InputManager;
      if (serviceType == typeof(IFileSystem)) return FileSystem;

      return null;
    }

    public void Tick(DeltaTime deltaTime)
    {
      InputManager.Update();
    }

    public void Dispose()
    {
      // no-op
    }
  }
}
