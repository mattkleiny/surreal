using OpenTK.Windowing.GraphicsLibraryFramework;
using Surreal.Input.Mouse;
using MouseButton = Surreal.Input.Mouse.MouseButton;

namespace Surreal.Internal.Input;

internal sealed class OpenTKMouseDevice(OpenTKWindow window) : IMouseDevice
{
  private readonly MouseState _mouseState = window.MouseState;
  private readonly OpenTKWindow _window = window;

  public event Action<MouseButton>? ButtonPressed;
  public event Action<MouseButton>? ButtonReleased;
  public event Action<Vector2>? Moved;

  public Vector2 Position => new(_mouseState.X, _mouseState.Y);
  public Vector2 NormalisedPosition => new(_mouseState.X / _window.Width, _mouseState.Y / _window.Height);
  public Vector2 DeltaPosition => new(_mouseState.X - _mouseState.PreviousX, _mouseState.Y - _mouseState.PreviousY);

  public bool IsCursorVisible
  {
    get => _window.IsCursorVisible;
    set => _window.IsCursorVisible = value;
  }

  public bool IsButtonDown(MouseButton button)
  {
    return _mouseState.IsButtonDown(Convert(button));
  }

  public bool IsButtonUp(MouseButton button)
  {
    return !_mouseState.IsButtonDown(Convert(button));
  }

  public bool IsButtonPressed(MouseButton button)
  {
    return _mouseState.IsButtonDown(Convert(button)) && !_mouseState.WasButtonDown(Convert(button));
  }

  public bool IsButtonReleased(MouseButton button)
  {
    return _mouseState.WasButtonDown(Convert(button)) && !_mouseState.IsButtonDown(Convert(button));
  }

  public void Update()
  {
    if (_window.IsFocused)
    {
      // press events
      if (IsButtonPressed(MouseButton.Left))
      {
        ButtonPressed?.Invoke(MouseButton.Left);
      }

      if (IsButtonPressed(MouseButton.Middle))
      {
        ButtonPressed?.Invoke(MouseButton.Middle);
      }

      if (IsButtonPressed(MouseButton.Right))
      {
        ButtonPressed?.Invoke(MouseButton.Right);
      }

      // release events
      if (IsButtonReleased(MouseButton.Left))
      {
        ButtonReleased?.Invoke(MouseButton.Left);
      }

      if (IsButtonReleased(MouseButton.Middle))
      {
        ButtonReleased?.Invoke(MouseButton.Middle);
      }

      if (IsButtonReleased(MouseButton.Right))
      {
        ButtonReleased?.Invoke(MouseButton.Right);
      }

      // movement events
      if (Math.Abs(_mouseState.X - _mouseState.PreviousX) > float.Epsilon ||
          Math.Abs(_mouseState.Y - _mouseState.PreviousY) > float.Epsilon)
      {
        Moved?.Invoke(DeltaPosition);
      }
    }
  }

  private static OpenTK.Windowing.GraphicsLibraryFramework.MouseButton Convert(MouseButton button)
  {
    switch (button)
    {
      case MouseButton.Left: return OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left;
      case MouseButton.Middle: return OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Middle;
      case MouseButton.Right: return OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Right;

      default:
        throw new ArgumentOutOfRangeException(nameof(button), button, "An unrecognized mouse button was requested.");
    }
  }
}
