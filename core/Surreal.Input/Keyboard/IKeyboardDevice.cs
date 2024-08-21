namespace Surreal.Input.Keyboard;

/// <summary>
/// An event that is raised when a keyboard key is pressed or released.
/// </summary>
public readonly record struct KeyPressEvent(Key Key, bool IsPressed) : IInputEvent;

/// <summary>
/// An event that is raised whilst a keyboard key is held.
/// </summary>
public readonly record struct KeyHeldEvent(Key Key) : IInputEvent;

/// <summary>
/// A keyboard <see cref="IInputDevice" />.
/// </summary>
public interface IKeyboardDevice : IInputDevice
{
  static IKeyboardDevice Null { get; } = new NullKeyboardDevice();

  Type IInputDevice.Type => typeof(IKeyboardDevice);

  event Action<Key> KeyPressed;
  event Action<Key> KeyReleased;

  bool IsKeyDown(Key key);
  bool IsKeyUp(Key key);
  bool IsKeyPressed(Key key);

  /// <summary>
  /// A no-op <see cref="IKeyboardDevice" />.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullKeyboardDevice : IKeyboardDevice
  {
    public event Action<Key>? KeyPressed;
    public event Action<Key>? KeyReleased;

    public IInputObservable Events => IInputObservable.Null;

    public bool IsKeyDown(Key key)
    {
      return false;
    }

    public bool IsKeyUp(Key key)
    {
      return !false;
    }

    public bool IsKeyPressed(Key key)
    {
      return false;
    }
  }
}
