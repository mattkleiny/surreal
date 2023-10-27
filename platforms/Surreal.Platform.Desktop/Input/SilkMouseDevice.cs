using Silk.NET.Input;
using Silk.NET.Windowing;
using Surreal.Input.Mouse;
using MouseButton = Surreal.Input.Mouse.MouseButton;

namespace Surreal.Input;

internal sealed class SilkMouseDevice(IWindow window, IMouse mouse) : IMouseDevice
{
  public Vector2 Position => mouse.Position;
  public Vector2 NormalisedPosition => mouse.Position / new Vector2(window.Size.X, window.Size.Y);
  public float ScrollAmount => mouse.ScrollWheels[0].Y;

  public event Action<MouseButton>? ButtonPressed;
  public event Action<MouseButton>? ButtonReleased;
  public event Action<Vector2>? Moved;

  public bool IsButtonDown(MouseButton button)
  {
    return mouse.IsButtonPressed(ConvertMouseButton(button));
  }

  public bool IsButtonUp(MouseButton button)
  {
    throw new NotImplementedException();
  }

  public bool IsButtonPressed(MouseButton button)
  {
    throw new NotImplementedException();
  }

  public bool IsButtonReleased(MouseButton button)
  {
    throw new NotImplementedException();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static Silk.NET.Input.MouseButton ConvertMouseButton(MouseButton button)
  {
    return button switch
    {
      MouseButton.Left => Silk.NET.Input.MouseButton.Left,
      MouseButton.Middle => Silk.NET.Input.MouseButton.Middle,
      MouseButton.Right => Silk.NET.Input.MouseButton.Right,

      _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
    };
  }
}
