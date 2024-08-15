using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Surreal.Input;

/// <summary>
/// A <see cref="IInputBackend"/> implementation that uses Silk.NET.
/// </summary>
internal sealed class SilkInputBackend(IWindow window, IInputContext context) : IInputBackend
{
  private readonly SilkKeyboardDevice? _keyboardDevice = context.Keyboards.Select(keyboard => new SilkKeyboardDevice(keyboard)).SingleOrDefault();
  private readonly SilkMouseDevice? _mouseDevice = context.Mice.Select(mouse => new SilkMouseDevice(window, mouse)).SingleOrDefault();
  private readonly SilkJoystickDevice? _joystickDevice = context.Joysticks.Select(joystick => new SilkJoystickDevice(joystick)).SingleOrDefault();
  private readonly SilkGamepadDevice? _gamepadDevice = context.Gamepads.Select(gamepad => new SilkGamepadDevice(gamepad)).SingleOrDefault();

  /// <summary>
  /// The underlying input devices.
  /// </summary>
  private IEnumerable<IInputDevice?> Devices => [_keyboardDevice, _mouseDevice, _joystickDevice, _gamepadDevice];

  /// <inheritdoc/>
  public IEnumerable<IInputDevice> CreateDevices()
  {
    return Devices.Where(it => it != null).Cast<IInputDevice>();
  }

  public void Update()
  {
    _keyboardDevice?.Update();
  }
}
