using Silk.NET.Input;
using Surreal.Input.Gamepad;

namespace Surreal.Input;

/// <summary>
/// A <see cref="IGamepadDevice"/> implementation that uses Silk.NET.
/// </summary>
internal sealed class SilkGamepadDevice : IGamepadDevice, IDisposable
{
  private readonly IGamepad _gamepad;
  private readonly InputEventSubject _events = new();

  public SilkGamepadDevice(IGamepad gamepad)
  {
    _gamepad = gamepad;

    gamepad.ButtonDown += OnButtonDown;
    gamepad.ButtonUp += OnButtonUp;
  }

  public event Action<GamepadButton>? ButtonPressed;
  public event Action<GamepadButton>? ButtonReleased;

  public IInputObservable Events => _events;

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

      GamepadButton.LeftStick => _gamepad.LeftThumbstickButton().Pressed,
      GamepadButton.RightStick => _gamepad.RightThumbstickButton().Pressed,

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

  private void OnButtonDown(IGamepad gamepad, Button silkButton)
  {
    var button = ConvertButton(silkButton.Name);

    ButtonPressed?.Invoke(button);
    _events.NotifyNext(new GamepadButtonEvent(button, IsPressed: true));
  }

  private void OnButtonUp(IGamepad gamepad, Button silkButton)
  {
    var button = ConvertButton(silkButton.Name);

    ButtonReleased?.Invoke(button);
    _events.NotifyNext(new GamepadButtonEvent(button, IsPressed: false));
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

  /// <summary>
  /// Converts a Silk.NET <see cref="ButtonName"/> to a <see cref="GamepadButton"/>.
  /// </summary>
  private static GamepadButton ConvertButton(ButtonName button) => button switch
  {
    ButtonName.A => GamepadButton.A,
    ButtonName.B => GamepadButton.B,
    ButtonName.X => GamepadButton.X,
    ButtonName.Y => GamepadButton.Y,
    ButtonName.LeftBumper => GamepadButton.LeftBumper,
    ButtonName.RightBumper => GamepadButton.RightBumper,
    ButtonName.Back => GamepadButton.Back,
    ButtonName.Start => GamepadButton.Start,
    ButtonName.Home => GamepadButton.Home,
    ButtonName.LeftStick => GamepadButton.LeftStick,
    ButtonName.RightStick => GamepadButton.RightStick,
    ButtonName.DPadUp => GamepadButton.DPadUp,
    ButtonName.DPadRight => GamepadButton.DPadRight,
    ButtonName.DPadDown => GamepadButton.DPadDown,
    ButtonName.DPadLeft => GamepadButton.DPadLeft,
    _ => GamepadButton.Unknown
  };
}
