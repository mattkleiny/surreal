using Surreal.Audio;
using Surreal.Compute;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Internal.Audio;
using Surreal.Internal.Compute;
using Surreal.Internal.Graphics;
using Surreal.Internal.Input;
using Surreal.Internal.Networking;
using Surreal.Networking.Transports;
using Surreal.Threading;
using Surreal.Timing;

namespace Surreal;

internal sealed class HeadlessPlatformHost : IHeadlessPlatformHost, IServiceModule
{
  public event Action<int, int> Resized = null!;

  public HeadlessAudioDevice      AudioDevice      { get; } = new();
  public HeadlessComputeDevice    ComputeDevice    { get; } = new();
  public HeadlessGraphicsDevice   GraphicsDevice   { get; } = new();
  public HeadlessInputManager     InputManager     { get; } = new();
  public HeadlessTransportFactory TransportFactory { get; } = new();

  public int  Width     => 1920;
  public int  Height    => 1080;
  public bool IsVisible => true;
  public bool IsFocused => true;
  public bool IsClosing => false;

  public IServiceModule Services   => this;
  public IDispatcher    Dispatcher { get; } = new ImmediateDispatcher();

  public IHeadlessKeyboardDevice Keyboard => InputManager.Keyboard;
  public IHeadlessMouseDevice    Mouse    => InputManager.Mouse;

  public void Tick(DeltaTime deltaTime)
  {
    InputManager.Update();
  }

  public void Dispose()
  {
    // no-op
  }

  void IServiceModule.RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton<IAudioDevice>(AudioDevice);
    services.AddSingleton<IComputeDevice>(ComputeDevice);
    services.AddSingleton<IGraphicsDevice>(GraphicsDevice);
    services.AddSingleton<IInputManager>(InputManager);
    services.AddSingleton<IKeyboardDevice>(InputManager.Keyboard);
    services.AddSingleton<IMouseDevice>(InputManager.Mouse);
    services.AddSingleton<IHeadlessKeyboardDevice>(InputManager.Keyboard);
    services.AddSingleton<IHeadlessMouseDevice>(InputManager.Mouse);
    services.AddSingleton<ITransportFactory>(TransportFactory);
  }
}
