namespace Surreal.Input.Gamepad;

/// <summary>
/// Possible gamepad buttons.
/// </summary>
public enum GamepadButton : byte
{
  // normal
  A,
  B,
  X,
  Y,

  // dpad
  DPadUp,
  DPadDown,
  DPadLeft,
  DPadRight,

  // bumper
  LeftBumper,
  RightBumper,

  // thumb sticks
  LeftStick,
  RightStick,

  // extra
  Back,
  Start,
  Home,
  Unknown,
}
