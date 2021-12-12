using Surreal.Audio;
using Surreal.Compute;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.Internal.Audio;
using Surreal.Internal.Compute;
using Surreal.Internal.Graphics;
using Surreal.Internal.Input;
using Surreal.IO;
using Surreal.Timing;

namespace Surreal;

internal sealed class HeadlessPlatformHost : IHeadlessPlatformHost, IServiceProvider
{
  public HeadlessAudioDevice    AudioDevice    { get; } = new();
  public HeadlessComputeDevice  ComputeDevice  { get; } = new();
  public HeadlessGraphicsDevice GraphicsDevice { get; } = new();
  public HeadlessInputManager   InputManager   { get; } = new();
  public LocalFileSystem        FileSystem     { get; } = new();

  public event Action<int, int> Resized = null!;

  public int  Width     => 1920;
  public int  Height    => 1080;
  public bool IsVisible => true;
  public bool IsFocused => true;
  public bool IsClosing => false;

  public IServiceProvider Services => this;

  public IHeadlessKeyboardDevice Keyboard => InputManager.Keyboard;
  public IHeadlessMouseDevice    Mouse    => InputManager.Mouse;

  object? IServiceProvider.GetService(Type serviceType)
  {
    if (serviceType == typeof(IAudioDevice)) return AudioDevice;
    if (serviceType == typeof(IComputeDevice)) return ComputeDevice;
    if (serviceType == typeof(IGraphicsDevice)) return GraphicsDevice;
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
