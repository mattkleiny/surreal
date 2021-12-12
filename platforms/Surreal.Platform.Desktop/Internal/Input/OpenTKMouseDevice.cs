using System.Numerics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Surreal.Input;
using Surreal.Input.Mouse;
using MouseButton = Surreal.Input.Mouse.MouseButton;

namespace Surreal.Internal.Input;

internal sealed class OpenTKMouseDevice : BufferedInputDevice<MouseState>, IMouseDevice
{
  private readonly OpenTKWindow window;

  public OpenTKMouseDevice(OpenTKWindow window)
  {
    this.window = window;

    UpdateState();
  }

  public event Action<MouseButton>? ButtonPressed;
  public event Action<MouseButton>? ButtonReleased;
  public event Action<Vector2>?     Moved;

  public Vector2 Position      => new(CurrentState.X, CurrentState.Y);
  public Vector2 DeltaPosition => new(CurrentState.X - PreviousState.X, CurrentState.Y - PreviousState.Y);

  public bool IsCursorVisible
  {
    get => window.IsCursorVisible;
    set => window.IsCursorVisible = value;
  }

  public bool IsButtonDown(MouseButton button)     => CurrentState.IsButtonDown(Convert(button));
  public bool IsButtonUp(MouseButton button)       => !CurrentState.IsButtonDown(Convert(button));
  public bool IsButtonPressed(MouseButton button)  => CurrentState.IsButtonDown(Convert(button)) && !PreviousState.IsButtonDown(Convert(button));
  public bool IsButtonReleased(MouseButton button) => PreviousState.IsButtonDown(Convert(button)) && !CurrentState.IsButtonDown(Convert(button));

  public override void Update()
  {
    if (window.IsFocused)
    {
      base.Update();

      // fire events, if necessary
      if (PreviousState != CurrentState)
      {
        // press events
        if (IsButtonPressed(MouseButton.Left)) ButtonPressed?.Invoke(MouseButton.Left);
        if (IsButtonPressed(MouseButton.Middle)) ButtonPressed?.Invoke(MouseButton.Middle);
        if (IsButtonPressed(MouseButton.Right)) ButtonPressed?.Invoke(MouseButton.Right);

        // release events
        if (IsButtonReleased(MouseButton.Left)) ButtonReleased?.Invoke(MouseButton.Left);
        if (IsButtonReleased(MouseButton.Middle)) ButtonReleased?.Invoke(MouseButton.Middle);
        if (IsButtonReleased(MouseButton.Right)) ButtonReleased?.Invoke(MouseButton.Right);

        // movement events
        if (Math.Abs(CurrentState.X - PreviousState.X) > float.Epsilon ||
            Math.Abs(CurrentState.Y - PreviousState.Y) > float.Epsilon)
        {
          Moved?.Invoke(DeltaPosition);
        }
      }
    }
  }

  protected override MouseState CaptureState()
  {
    return window.GetMouseState();
  }

  private static OpenTK.Windowing.GraphicsLibraryFramework.MouseButton Convert(MouseButton button)
  {
    switch (button)
    {
      case MouseButton.Left:   return OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left;
      case MouseButton.Middle: return OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Middle;
      case MouseButton.Right:  return OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Right;

      default:
        throw new ArgumentOutOfRangeException(nameof(button), button, "An unrecognized mouse button was requested.");
    }
  }
}
