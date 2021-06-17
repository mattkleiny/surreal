using System;
using OpenTK.Input;
using Surreal.Input;
using Surreal.Input.Mouse;
using Surreal.Mathematics.Linear;
using MouseButton = Surreal.Input.Mouse.MouseButton;

namespace Surreal.Platform.Internal.Input {
  internal sealed class OpenTKMouseDevice : BufferedInputDevice<MouseState>, IMouseDevice {
    private readonly IDesktopWindow window;

    public OpenTKMouseDevice(IDesktopWindow window) {
      this.window = window;
    }

    public event Action<MouseButton>? ButtonPressed;
    public event Action<MouseButton>? ButtonReleased;
    public event Action<Vector2I>?    Moved;

    public Vector2I Position      => new(CurrentState.X, CurrentState.Y);
    public Vector2I DeltaPosition => new(CurrentState.X - PreviousState.X, CurrentState.Y - PreviousState.Y);

    public bool IsLockedToWindow { get; set; } = false;

    public bool IsCursorVisible {
      get => window.IsCursorVisible;
      set => window.IsCursorVisible = value;
    }

    public bool IsButtonDown(MouseButton button)     => CurrentState.IsButtonDown(Convert(button));
    public bool IsButtonUp(MouseButton button)       => CurrentState.IsButtonUp(Convert(button));
    public bool IsButtonPressed(MouseButton button)  => CurrentState.IsButtonDown(Convert(button)) && PreviousState.IsButtonUp(Convert(button));
    public bool IsButtonReleased(MouseButton button) => PreviousState.IsButtonDown(Convert(button)) && CurrentState.IsButtonUp(Convert(button));

    public override void Update() {
      if (window.IsFocused) {
        base.Update();

        // fire events, if necessary
        if (PreviousState != CurrentState) {
          // press events
          if (IsButtonPressed(MouseButton.Left)) ButtonPressed?.Invoke(MouseButton.Left);
          if (IsButtonPressed(MouseButton.Middle)) ButtonPressed?.Invoke(MouseButton.Middle);
          if (IsButtonPressed(MouseButton.Right)) ButtonPressed?.Invoke(MouseButton.Right);

          // release events
          if (IsButtonReleased(MouseButton.Left)) ButtonReleased?.Invoke(MouseButton.Left);
          if (IsButtonReleased(MouseButton.Middle)) ButtonReleased?.Invoke(MouseButton.Middle);
          if (IsButtonReleased(MouseButton.Right)) ButtonReleased?.Invoke(MouseButton.Right);

          // movement events
          if (CurrentState.X != PreviousState.X || CurrentState.Y != PreviousState.Y) {
            Moved?.Invoke(DeltaPosition);
          }

          if (IsLockedToWindow) {
            Mouse.SetPosition(window.Width / 2f, window.Height / 2f);
          }
        }
      }
    }

    protected override MouseState CaptureState() => Mouse.GetState();

    private static OpenTK.Input.MouseButton Convert(MouseButton button) {
      switch (button) {
        case MouseButton.Left:   return OpenTK.Input.MouseButton.Left;
        case MouseButton.Middle: return OpenTK.Input.MouseButton.Middle;
        case MouseButton.Right:  return OpenTK.Input.MouseButton.Right;

        default:
          throw new ArgumentOutOfRangeException(nameof(button), button, "An unrecognized mouse button was requested.");
      }
    }
  }
}