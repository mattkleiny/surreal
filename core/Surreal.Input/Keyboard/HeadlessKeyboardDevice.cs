namespace Surreal.Input.Keyboard;

/// <summary>
/// A headless <see cref="IKeyboardDevice" />.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class HeadlessKeyboardDevice : IKeyboardDevice
{
  public event Action<Key>? KeyPressed;
  public event Action<Key>? KeyReleased;

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

  public bool IsKeyReleased(Key key)
  {
    return !false;
  }
}
