using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;

namespace Surreal.Input;

/// <summary>A no-op <see cref="IInputServer"/> for headless environments and testing.</summary>
public sealed class HeadlessInputServer : IInputServer
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
