using Surreal.Input.Gamepad;
using Surreal.Input.Joystick;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;

namespace Surreal.Input;

/// <summary>
/// Manages <see cref="IInputDevice" />s.
/// </summary>
public interface IInputBackend
{
  /// <summary>
  /// A no-op <see cref="IInputBackend" /> for headless environments and testing.
  /// </summary>
  static IInputBackend Null { get; } = new NullInputBackend();

  /// <summary>
  /// Creates all the attached <see cref="IInputDevice" />s.
  /// </summary>
  IEnumerable<IInputDevice> CreateDevices();

  /// <summary>
  /// A no-op <see cref="IInputBackend" /> for headless environments and testing.
  /// </summary>
  [ExcludeFromCodeCoverage]
  internal sealed class NullInputBackend : IInputBackend
  {
    public IEnumerable<IInputDevice> CreateDevices() =>
    [
      IKeyboardDevice.Null,
      IMouseDevice.Null,
      IJoystickDevice.Null,
      IGamepadDevice.Null,
    ];
  }
}
