using System;
using System.Collections.Generic;
using Surreal.Input.Mouse;
using Surreal.Mathematics.Linear;

namespace Surreal.Platform.Internal.Input {
  internal sealed class HeadlessMouseDevice : IHeadlessMouseDevice {
    private readonly HashSet<MouseButton> pressedButtons = new();

    public event Action<MouseButton> ButtonPressed  = null!;
    public event Action<MouseButton> ButtonReleased = null!;
    public event Action<Point2>    Moved          = null!;

    public bool this[MouseButton button] {
      get => pressedButtons.Contains(button);
      set {
        if (value) {
          pressedButtons.Add(button);
        }
        else {
          pressedButtons.Remove(button);
        }
      }
    }

    public Point2 Position      { get; set; } = new(0, 0);
    public Point2 DeltaPosition => new(0, 0);

    public bool IsLockedToWindow { get; set; } = false;
    public bool IsCursorVisible  { get; set; } = true;

    public bool IsButtonDown(MouseButton button)     => false;
    public bool IsButtonUp(MouseButton button)       => false;
    public bool IsButtonPressed(MouseButton button)  => false;
    public bool IsButtonReleased(MouseButton button) => false;

    public void Update() {
    }
  }
}