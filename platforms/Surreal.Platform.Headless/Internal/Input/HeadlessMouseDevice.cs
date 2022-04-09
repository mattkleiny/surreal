using Surreal.Input.Mouse;

namespace Surreal.Internal.Input;

internal sealed class HeadlessMouseDevice : IHeadlessMouseDevice
{
  private readonly HashSet<MouseButton> pressedButtons = new();

  public event Action<MouseButton>? ButtonPressed;
  public event Action<MouseButton>? ButtonReleased;
  public event Action<Vector2>?     Moved;

  public bool this[MouseButton button]
  {
    get => pressedButtons.Contains(button);
    set
    {
      if (value)
      {
        pressedButtons.Add(button);
      }
      else
      {
        pressedButtons.Remove(button);
      }
    }
  }

  public Vector2 Position      { get; set; } = new(0, 0);
  public Vector2 DeltaPosition => new(0, 0);

  public bool IsCursorVisible { get; set; } = true;

  public bool IsButtonDown(MouseButton button) => false;
  public bool IsButtonUp(MouseButton button) => false;
  public bool IsButtonPressed(MouseButton button) => false;
  public bool IsButtonReleased(MouseButton button) => false;

  public void Update()
  {
  }
}
