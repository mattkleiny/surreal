namespace Surreal.Input.Keyboard;

/// <summary>
/// A keyboard <see cref="IInputDevice" />.
/// </summary>
public interface IKeyboardDevice : IInputDevice
{
  Type IInputDevice.Type => typeof(IKeyboardDevice);

  event Action<Key> KeyPressed;
  event Action<Key> KeyReleased;

  bool IsKeyDown(Key key);
  bool IsKeyUp(Key key);
  bool IsKeyPressed(Key key);
}
