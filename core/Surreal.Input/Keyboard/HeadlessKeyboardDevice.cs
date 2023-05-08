namespace Surreal.Input.Keyboard;

/// <summary>
/// A headless <see cref="IKeyboardDevice" />.
/// </summary>
public sealed class HeadlessKeyboardDevice : IKeyboardDevice
{
  private readonly HashSet<Key> _pressedKeys = new();

  public bool this[Key key]
  {
    get => _pressedKeys.Contains(key);
    set
    {
      if (value)
      {
        _pressedKeys.Add(key);
        KeyPressed?.Invoke(key);
      }
      else
      {
        _pressedKeys.Remove(key);
        KeyReleased?.Invoke(key);
      }
    }
  }

  public event Action<Key>? KeyPressed;
  public event Action<Key>? KeyReleased;

  public bool IsKeyDown(Key key)
  {
    return _pressedKeys.Contains(key);
  }

  public bool IsKeyUp(Key key)
  {
    return !_pressedKeys.Contains(key);
  }

  public bool IsKeyPressed(Key key)
  {
    return _pressedKeys.Contains(key);
  }

  public bool IsKeyReleased(Key key)
  {
    return !_pressedKeys.Contains(key);
  }
}
