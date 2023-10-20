using Silk.NET.Input;
using Surreal.Input.Keyboard;
using Key = Surreal.Input.Keyboard.Key;

namespace Surreal.Internal.Input;

internal sealed class SilkKeyboardDevice(IKeyboard keyboard) : IKeyboardDevice
{
  public event Action<Key>? KeyPressed;
  public event Action<Key>? KeyReleased;

  public bool IsKeyDown(Key key)
  {
    throw new NotImplementedException();
  }

  public bool IsKeyUp(Key key)
  {
    throw new NotImplementedException();
  }

  public bool IsKeyPressed(Key key)
  {
    throw new NotImplementedException();
  }

  public bool IsKeyReleased(Key key)
  {
    throw new NotImplementedException();
  }
}
