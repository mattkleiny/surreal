﻿namespace Surreal.Input.Mouse;

/// <summary>
/// A mouse <see cref="IInputDevice" />.
/// </summary>
public interface IMouseDevice : IInputDevice
{
  Type IInputDevice.DeviceType => typeof(IMouseDevice);

  Vector2 Position { get; }
  Vector2 NormalisedPosition { get; }
  float ScrollAmount { get; }

  event Action<MouseButton> ButtonPressed;
  event Action<MouseButton> ButtonReleased;

  event Action<Vector2> Moved;

  bool IsButtonDown(MouseButton button);
  bool IsButtonUp(MouseButton button);
  bool IsButtonPressed(MouseButton button);
  bool IsButtonReleased(MouseButton button);
}
