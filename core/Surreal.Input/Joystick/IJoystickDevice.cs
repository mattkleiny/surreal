namespace Surreal.Input.Joystick;

/// <summary>
/// Represents a joystick device.
/// </summary>
public interface IJoystickDevice : IInputDevice
{
  static IJoystickDevice Null { get; } = new NullJoystickDevice();

  Type IInputDevice.Type => typeof(IJoystickDevice);

  /// <summary>
  /// The value of the left stick's normalized axes.
  /// </summary>
  Vector2 LeftSick { get; }

  /// <summary>
  /// The value of the right stick's normalized axes.
  /// </summary>
  Vector2 RightSick { get; }

  /// <summary>
  /// A no-op <see cref="IJoystickDevice"/> implementation.
  /// </summary>
  private sealed class NullJoystickDevice : IJoystickDevice
  {
    public Vector2 LeftSick => Vector2.Zero;
    public Vector2 RightSick => Vector2.Zero;
  }
}
