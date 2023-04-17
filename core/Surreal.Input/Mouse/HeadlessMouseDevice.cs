namespace Surreal.Input.Mouse;

/// <summary>
/// A headless <see cref="IMouseDevice" />.
/// </summary>
public sealed class HeadlessMouseDevice : IMouseDevice
{
  private readonly HashSet<MouseButton> _pressedButtons = new();

  public bool this[MouseButton button]
  {
    get => _pressedButtons.Contains(button);
    set
    {
      if (value)
      {
        _pressedButtons.Add(button);
      }
      else
      {
        _pressedButtons.Remove(button);
      }
    }
  }

  public event Action<MouseButton>? ButtonPressed;
  public event Action<MouseButton>? ButtonReleased;
  public event Action<Vector2>? Moved;

  public Vector2 Position { get; set; } = new(0, 0);
  public Vector2 NormalisedPosition => Position;
  public Vector2 DeltaPosition => new(0, 0);

  public bool IsCursorVisible { get; set; } = true;

  public bool IsButtonDown(MouseButton button)
  {
    return false;
  }

  public bool IsButtonUp(MouseButton button)
  {
    return false;
  }

  public bool IsButtonPressed(MouseButton button)
  {
    return false;
  }

  public bool IsButtonReleased(MouseButton button)
  {
    return false;
  }

  public void Update()
  {
  }
}
