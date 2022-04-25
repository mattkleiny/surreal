namespace Surreal.Input.Keyboard;

/// <summary>A headless <see cref="IKeyboardDevice"/>.</summary>
public sealed class HeadlessKeyboardDevice : IKeyboardDevice
{
  private readonly HashSet<Key> pressedKeys = new();

  public event Action<Key>? KeyPressed;
  public event Action<Key>? KeyReleased;

  public bool IsKeyDown(Key key) => pressedKeys.Contains(key);
  public bool IsKeyUp(Key key) => !pressedKeys.Contains(key);
  public bool IsKeyPressed(Key key) => pressedKeys.Contains(key);
  public bool IsKeyReleased(Key key) => !pressedKeys.Contains(key);

  public bool this[Key key]
  {
    get => pressedKeys.Contains(key);
    set
    {
      if (value)
      {
        pressedKeys.Add(key);
      }
      else
      {
        pressedKeys.Remove(key);
      }
    }
  }

  public void Update()
  {
  }
}
