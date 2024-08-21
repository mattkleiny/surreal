using Silk.NET.Input;
using Silk.NET.Windowing;
using Surreal.Input.Mouse;
using MouseButton = Surreal.Input.Mouse.MouseButton;

namespace Surreal.Input;

/// <summary>
/// A <see cref="IMouseDevice"/> implementation that uses Silk.NET.
/// </summary>
internal sealed class SilkMouseDevice : IMouseDevice, IDisposable
{
  private readonly IWindow _window;
  private readonly IMouse _mouse;
  private readonly InputEventSubject _events = new();

  public SilkMouseDevice(IWindow window, IMouse mouse)
  {
    _window = window;
    _mouse = mouse;

    _mouse.MouseDown += OnMouseDown;
    _mouse.MouseUp += OnMouseUp;
  }

  public event Action<MouseButton>? ButtonPressed;
  public event Action<MouseButton>? ButtonReleased;

  public IInputObservable Events => _events;

  public Vector2 Position => _mouse.Position;
  public Vector2 NormalisedPosition => _mouse.Position / new Vector2(_window.Size.X, _window.Size.Y);

  public float ScrollAmount => _mouse.ScrollWheels[0].Y;

  public bool IsButtonDown(MouseButton button)
  {
    return _mouse.IsButtonPressed(ConvertButtonToSilk(button));
  }

  public bool IsButtonUp(MouseButton button)
  {
    return !IsButtonDown(button);
  }

  public void Dispose()
  {
    _mouse.MouseDown -= OnMouseDown;
    _mouse.MouseUp -= OnMouseUp;
  }

  private void OnMouseDown(IMouse mouse, Silk.NET.Input.MouseButton silkButton)
  {
    var button = ConvertButtonFromSilk(silkButton);

    ButtonPressed?.Invoke(button);

    _events.NotifyNext(new MouseButtonEvent(button, IsPressed: true));
  }

  private void OnMouseUp(IMouse mouse, Silk.NET.Input.MouseButton silkButton)
  {
    var button = ConvertButtonFromSilk(silkButton);

    ButtonReleased?.Invoke(button);

    _events.NotifyNext(new MouseButtonEvent(button, IsPressed: false));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static Silk.NET.Input.MouseButton ConvertButtonToSilk(MouseButton button)
  {
    return button switch
    {
      MouseButton.Left => Silk.NET.Input.MouseButton.Left,
      MouseButton.Middle => Silk.NET.Input.MouseButton.Middle,
      MouseButton.Right => Silk.NET.Input.MouseButton.Right,

      _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
    };
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static MouseButton ConvertButtonFromSilk(Silk.NET.Input.MouseButton button)
  {
    return button switch
    {
      Silk.NET.Input.MouseButton.Left => MouseButton.Left,
      Silk.NET.Input.MouseButton.Middle => MouseButton.Middle,
      Silk.NET.Input.MouseButton.Right => MouseButton.Right,

      _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
    };
  }
}
