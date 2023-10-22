using Silk.NET.Input;
using Surreal.Input.Mouse;
using MouseButton = Surreal.Input.Mouse.MouseButton;

namespace Surreal.Input;

internal sealed class SilkMouseDevice(IMouse mouse) : IMouseDevice
{
  public Vector2 Position { get; }
  public Vector2 NormalisedPosition { get; }
  public Vector2 DeltaPosition { get; }

  public bool IsCursorVisible { get; set; }

  public event Action<MouseButton>? ButtonPressed;
  public event Action<MouseButton>? ButtonReleased;
  public event Action<Vector2>? Moved;

  public bool IsButtonDown(MouseButton button)
  {
    throw new NotImplementedException();
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
}
