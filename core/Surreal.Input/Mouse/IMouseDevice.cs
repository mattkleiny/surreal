namespace Surreal.Input.Mouse;

/// <summary>
/// A mouse <see cref="IInputDevice" />.
/// </summary>
public interface IMouseDevice : IInputDevice
{
  static IMouseDevice Null { get; } = new NullMouseDevice();

  Type IInputDevice.Type => typeof(IMouseDevice);

  Vector2 Position { get; }
  Vector2 NormalisedPosition { get; }
  float ScrollAmount { get; }

  event Action<MouseButton> ButtonPressed;
  event Action<MouseButton> ButtonReleased;

  bool IsButtonDown(MouseButton button);
  bool IsButtonUp(MouseButton button);

  /// <summary>
  /// A no-op <see cref="IMouseDevice" />.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullMouseDevice : IMouseDevice
  {
    public event Action<MouseButton>? ButtonPressed;
    public event Action<MouseButton>? ButtonReleased;

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
  }
}
