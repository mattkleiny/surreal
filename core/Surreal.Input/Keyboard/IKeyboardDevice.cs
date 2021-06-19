using System;

namespace Surreal.Input.Keyboard
{
  public interface IKeyboardDevice : IInputDevice
  {
    event Action<Key> KeyPressed;
    event Action<Key> KeyReleased;

    bool IsKeyDown(Key key);
    bool IsKeyUp(Key key);
    bool IsKeyPressed(Key key);
    bool IsKeyReleased(Key key);
  }
}