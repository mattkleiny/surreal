using System;
using Surreal.Input.Mouse;
using Surreal.Mathematics.Linear;

namespace Surreal.Platform.Internal.Input
{
  internal sealed class HeadlessMouseDevice : IHeadlessMouseDevice
  {
    public event Action<MouseButton> ButtonPressed;
    public event Action<MouseButton> ButtonReleased;
    public event Action<Vector2I>      Moved;

    public Vector2I Position      { get; set; } = new Vector2I(0, 0);
    public Vector2I DeltaPosition => new Vector2I(0, 0);

    public bool IsLockedToWindow { get; set; } = false;
    public bool IsCursorVisible  { get; set; } = true;

    public bool IsButtonDown(MouseButton button)     => false;
    public bool IsButtonUp(MouseButton button)       => false;
    public bool IsButtonPressed(MouseButton button)  => false;
    public bool IsButtonReleased(MouseButton button) => false;

    public void Update()
    {
    }
  }
}
