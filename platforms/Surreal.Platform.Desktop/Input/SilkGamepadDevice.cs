using Silk.NET.Input;
using Surreal.Input.Gamepad;

namespace Surreal.Input;

/// <summary>
/// A <see cref="IGamepadDevice"/> implementation that uses Silk.NET.
/// </summary>
internal sealed class SilkGamepadDevice : IGamepadDevice, IDisposable
{
  private readonly IGamepad _gamepad;

  public SilkGamepadDevice(IGamepad gamepad)
  {
    _gamepad = gamepad;

    gamepad.ButtonDown += OnButtonDown;
    gamepad.ButtonUp += OnButtonUp;
  }

  public event Action<GamepadButton>? ButtonPressed;
  public event Action<GamepadButton>? ButtonReleased;

  public Vector2 LeftSick
  {
    get
    {
      if (!TryGetThumbstick(0, out var thumbstick))
      {
        return Vector2.Zero;
      }

      return new Vector2(thumbstick.X, thumbstick.Y);
    }
  }

  public Vector2 RightSick
  {
    get
    {
      if (!TryGetThumbstick(0, out var thumbstick))
      {
        return Vector2.Zero;
      }

      return new Vector2(thumbstick.X, thumbstick.Y);
    }
  }

  public bool IsButtonDown(GamepadButton button)
  {
    return button switch
    {
      GamepadButton.A => _gamepad.A().Pressed,
      GamepadButton.B => _gamepad.B().Pressed,
      GamepadButton.X => _gamepad.X().Pressed,
      GamepadButton.Y => _gamepad.Y().Pressed,

      GamepadButton.DPadUp => _gamepad.DPadUp().Pressed,
      GamepadButton.DPadDown => _gamepad.DPadDown().Pressed,
      GamepadButton.DPadLeft => _gamepad.DPadLeft().Pressed,
      GamepadButton.DPadRight => _gamepad.DPadRight().Pressed,

      GamepadButton.LeftBumper => _gamepad.LeftBumper().Pressed,
      GamepadButton.RightBumper => _gamepad.RightBumper().Pressed,

      GamepadButton.LeftThumbstick => _gamepad.LeftThumbstickButton().Pressed,
      GamepadButton.RightThumbstick => _gamepad.RightThumbstickButton().Pressed,

      GamepadButton.Back => _gamepad.Back().Pressed,
      GamepadButton.Start => _gamepad.Start().Pressed,

      _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
    };
  }

  public bool IsButtonUp(GamepadButton button)
  {
    return !IsButtonDown(button);
  }

  public void Dispose()
  {
    _gamepad.ButtonDown -= OnButtonDown;
    _gamepad.ButtonUp -= OnButtonUp;
  }

  private void OnButtonDown(IGamepad gamepad, Button button)
  {
    // TODO: implement me
  }

  private void OnButtonUp(IGamepad gamepad, Button button)
  {
    // TODO: implement me
  }

  /// <summary>
  /// Attempts to get the thumbstick at the specified index.
  /// </summary>
  private bool TryGetThumbstick(int index, out Thumbstick result)
  {
    if (index < 0 || index >= _gamepad.Thumbsticks.Count)
    {
      result = default;
      return false;
    }

    result = _gamepad.Thumbsticks[index];
    return true;
  }
}
