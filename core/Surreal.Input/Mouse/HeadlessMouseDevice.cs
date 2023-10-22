namespace Surreal.Input.Mouse;

/// <summary>
/// A headless <see cref="IMouseDevice" />.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class HeadlessMouseDevice : IMouseDevice
{
  public event Action<MouseButton>? ButtonPressed;
  public event Action<MouseButton>? ButtonReleased;
  public event Action<Vector2>? Moved;

  public Vector2 Position => Vector2.Zero;
  public Vector2 NormalisedPosition => Position;
  public float ScrollAmount => 0f;

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
}
