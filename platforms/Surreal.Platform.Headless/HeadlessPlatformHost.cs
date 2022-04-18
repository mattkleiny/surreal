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
using Surreal.Threading;
using Surreal.Timing;

namespace Surreal;

internal sealed class HeadlessPlatformHost : IHeadlessPlatformHost, IServiceModule
{
  public event Action<int, int> Resized = null!;

  public HeadlessAudioServer    AudioServer    { get; } = new();
  public HeadlessComputeServer  ComputeServer  { get; } = new();
  public HeadlessGraphicsServer GraphicsServer { get; } = new();
  public HeadlessInputServer    InputServer    { get; } = new();

  public int  Width     => 1920;
  public int  Height    => 1080;
  public bool IsVisible => true;
  public bool IsFocused => true;
  public bool IsClosing => false;

  public IServiceModule Services   => this;
  public IDispatcher    Dispatcher { get; } = new ImmediateDispatcher();

  public IHeadlessKeyboardDevice Keyboard => InputServer.Keyboard;
  public IHeadlessMouseDevice    Mouse    => InputServer.Mouse;

  public void Tick(DeltaTime deltaTime)
  {
    InputServer.Update();
  }

  public void Dispose()
  {
    // no-op
  }

  void IServiceModule.RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton<IAudioServer>(AudioServer);
    services.AddSingleton<IComputeServer>(ComputeServer);
    services.AddSingleton<IGraphicsServer>(GraphicsServer);
    services.AddSingleton<IInputServer>(InputServer);
    services.AddSingleton<IKeyboardDevice>(InputServer.Keyboard);
    services.AddSingleton<IMouseDevice>(InputServer.Mouse);
    services.AddSingleton<IHeadlessKeyboardDevice>(InputServer.Keyboard);
    services.AddSingleton<IHeadlessMouseDevice>(InputServer.Mouse);
  }
}
