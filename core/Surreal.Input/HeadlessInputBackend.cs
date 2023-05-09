using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;

namespace Surreal.Input;

/// <summary>
/// A no-op <see cref="IInputBackend" /> for headless environments and testing.
/// </summary>
public sealed class HeadlessInputBackend : IInputBackend
{
  private readonly List<IInputDevice> _devices = new();

  public HeadlessInputBackend()
  {
    _devices.Add(Keyboard);
    _devices.Add(Mouse);
  }

  public HeadlessKeyboardDevice Keyboard { get; } = new();
  public HeadlessMouseDevice Mouse { get; } = new();

  public IEnumerable<IInputDevice> Devices => _devices;
}
