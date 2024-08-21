namespace Surreal.Input.Gamepad;

/// <summary>
/// An event that is raised when a gamepad button is pressed or released.
/// </summary>
public readonly record struct GamepadButtonEvent(GamepadButton Button, bool IsPressed) : IInputEvent;

/// <summary>
/// Represents a gamepad device.
/// </summary>
public interface IGamepadDevice : IInputDevice
{
  static IGamepadDevice Null { get; } = new NullGamepadDevice();

  Type IInputDevice.Type => typeof(IGamepadDevice);

  public event Action<GamepadButton>? ButtonPressed;
  public event Action<GamepadButton>? ButtonReleased;

  /// <summary>
  /// The value of the left stick's normalized axes.
  /// </summary>
  Vector2 LeftSick { get; }

  /// <summary>
  /// The value of the right stick's normalized axes.
  /// </summary>
  Vector2 RightSick { get; }

  /// <summary>
  /// Is the specified button pressed?
  /// </summary>
  bool IsButtonDown(GamepadButton button);

  /// <summary>
  /// Is the specified button released?
  /// </summary>
  bool IsButtonUp(GamepadButton button);

  /// <summary>
  /// A no-op <see cref="IGamepadDevice"/> implementation.
  /// </summary>
  private sealed class NullGamepadDevice : IGamepadDevice
  {
    public event Action<GamepadButton>? ButtonPressed;
    public event Action<GamepadButton>? ButtonReleased;

    public IInputObservable Events => IInputObservable.Null;

    public Vector2 LeftSick => Vector2.Zero;
    public Vector2 RightSick => Vector2.Zero;

    public bool IsButtonDown(GamepadButton button)
    {
      return false;
    }

    public bool IsButtonUp(GamepadButton button)
    {
      return true;
    }
  }
}
