using System;
using Surreal.Audio;
using Surreal.Compute;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.IO;
using Surreal.IO.VFS;
using Surreal.Mathematics.Timing;
using Surreal.Platform.Internal.Audio;
using Surreal.Platform.Internal.Compute;
using Surreal.Platform.Internal.Graphics;
using Surreal.Platform.Internal.Input;

namespace Surreal.Platform {
  internal sealed class HeadlessPlatformHost : IHeadlessPlatformHost, IServiceProvider {
    public HeadlessAudioDevice    AudioDevice    { get; } = new HeadlessAudioDevice();
    public HeadlessComputeDevice  ComputeDevice  { get; } = new HeadlessComputeDevice();
    public HeadlessGraphicsDevice GraphicsDevice { get; } = new HeadlessGraphicsDevice();
    public HeadlessInputManager   InputManager   { get; } = new HeadlessInputManager();
    public LocalFileSystem        FileSystem     { get; } = new LocalFileSystem();

    public event Action<int, int> Resized = null!;

    public int  Width     { get; } = 1920;
    public int  Height    { get; } = 1080;
    public bool IsVisible { get; } = true;
    public bool IsFocused { get; } = true;
    public bool IsClosing { get; } = false;

    public IServiceProvider Services => this;

    public IHeadlessKeyboardDevice Keyboard => InputManager.Keyboard;
    public IHeadlessMouseDevice    Mouse    => InputManager.Mouse;

    object? IServiceProvider.GetService(Type serviceType) {
      if (serviceType == typeof(IAudioDevice)) return AudioDevice;
      if (serviceType == typeof(IComputeDevice)) return ComputeDevice;
      if (serviceType == typeof(IGraphicsDevice)) return GraphicsDevice;
      if (serviceType == typeof(IInputManager)) return InputManager;
      if (serviceType == typeof(IFileSystem)) return FileSystem;

      return null;
    }

    public void Tick(DeltaTime deltaTime) {
      InputManager.Update();
    }

    public void Dispose() {
      // no-op
    }
  }
}