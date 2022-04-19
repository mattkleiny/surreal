using OpenTK.Windowing.GraphicsLibraryFramework;
using Surreal.Input.Mouse;
using MouseButton = Surreal.Input.Mouse.MouseButton;

namespace Surreal.Internal.Input;

internal sealed class OpenTKMouseDevice : IMouseDevice
{
  private readonly OpenTKWindow window;
  private readonly MouseState mouseState;

  public OpenTKMouseDevice(OpenTKWindow window)
  {
    this.window = window;

    mouseState = window.MouseState;
  }

  public event Action<MouseButton>? ButtonPressed;
  public event Action<MouseButton>? ButtonReleased;
  public event Action<Vector2>?     Moved;

  public Vector2 Position      => new(mouseState.X, mouseState.Y);
  public Vector2 DeltaPosition => new(mouseState.X - mouseState.PreviousX, mouseState.Y - mouseState.PreviousY);

  public bool IsCursorVisible
  {
    get => window.IsCursorVisible;
    set => window.IsCursorVisible = value;
  }

  public bool IsButtonDown(MouseButton button) => mouseState.IsButtonDown(Convert(button));
  public bool IsButtonUp(MouseButton button) => !mouseState.IsButtonDown(Convert(button));
  public bool IsButtonPressed(MouseButton button) => mouseState.IsButtonDown(Convert(button)) && !mouseState.WasButtonDown(Convert(button));
  public bool IsButtonReleased(MouseButton button) => mouseState.WasButtonDown(Convert(button)) && !mouseState.IsButtonDown(Convert(button));

  public void Update()
  {
    if (window.IsFocused)
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
      if (Math.Abs(mouseState.X - mouseState.PreviousX) > float.Epsilon ||
          Math.Abs(mouseState.Y - mouseState.PreviousY) > float.Epsilon)
      {
        Moved?.Invoke(DeltaPosition);
      }
    }
  }

  private static global::OpenTK.Windowing.GraphicsLibraryFramework.MouseButton Convert(MouseButton button)
  {
    switch (button)
    {
      case MouseButton.Left:   return global::OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left;
      case MouseButton.Middle: return global::OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Middle;
      case MouseButton.Right:  return global::OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Right;

      default:
        throw new ArgumentOutOfRangeException(nameof(button), button, "An unrecognized mouse button was requested.");
    }
  }
}
