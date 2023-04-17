using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;

namespace Surreal.Input;

/// <summary>
/// A no-op <see cref="IInputServer" /> for headless environments and testing.
/// </summary>
public sealed class HeadlessInputServer : IInputServer
{
  private readonly List<IInputDevice> _devices = new();

  public HeadlessInputServer()
  {
    _devices.Add(Keyboard);
    _devices.Add(Mouse);
  }

  public HeadlessKeyboardDevice Keyboard { get; } = new();
  public HeadlessMouseDevice Mouse { get; } = new();

  public IEnumerable<IInputDevice> Devices => _devices;

  public void Update()
  {
    Keyboard.Update();
    Mouse.Update();
  }
}
