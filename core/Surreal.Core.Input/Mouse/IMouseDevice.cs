﻿using System;
using Surreal.Mathematics.Linear;

namespace Surreal.Input.Mouse {
  public interface IMouseDevice : IInputDevice {
    event Action<MouseButton> ButtonPressed;
    event Action<MouseButton> ButtonReleased;

    event Action<Point2> Moved;

    Point2 Position      { get; }
    Point2 DeltaPosition { get; }

    bool IsLockedToWindow { get; set; }
    bool IsCursorVisible  { get; set; }

    bool IsButtonDown(MouseButton button);
    bool IsButtonUp(MouseButton button);
    bool IsButtonPressed(MouseButton button);
    bool IsButtonReleased(MouseButton button);
  }
}