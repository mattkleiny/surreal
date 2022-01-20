using Surreal.Input;

namespace Surreal.Internal.Input;

internal sealed class HeadlessInputServer : IInputServer
{
  private readonly List<IInputDevice> devices = new();

  public HeadlessInputServer()
  {
    devices.Add(Keyboard);
    devices.Add(Mouse);
  }

  public IEnumerable<IInputDevice> Devices => devices;

  public HeadlessKeyboardDevice Keyboard { get; } = new();
  public HeadlessMouseDevice    Mouse    { get; } = new();

  public void Update()
  {
    Keyboard.Update();
    Mouse.Update();
  }
}
