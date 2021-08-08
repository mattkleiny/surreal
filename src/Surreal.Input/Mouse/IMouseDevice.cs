using System;
using System.Numerics;

namespace Surreal.Input.Mouse
{
  public interface IMouseDevice : IInputDevice
  {
    event Action<MouseButton> ButtonPressed;
    event Action<MouseButton> ButtonReleased;

    event Action<Vector2> Moved;

    Vector2 Position      { get; }
    Vector2 DeltaPosition { get; }

    bool IsCursorVisible { get; set; }

    bool IsButtonDown(MouseButton button);
    bool IsButtonUp(MouseButton button);
    bool IsButtonPressed(MouseButton button);
    bool IsButtonReleased(MouseButton button);
  }
}
