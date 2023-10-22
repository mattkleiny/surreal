using Silk.NET.Input;
using Surreal.Input.Keyboard;
using Key = Surreal.Input.Keyboard.Key;

namespace Surreal.Input;

internal sealed class SilkKeyboardDevice(IKeyboard keyboard) : IKeyboardDevice
{
  public event Action<Key>? KeyPressed;
  public event Action<Key>? KeyReleased;

  public bool IsKeyDown(Key key)
  {
    return keyboard.IsKeyPressed(ConvertKey(key));
  }

  public bool IsKeyUp(Key key)
  {
    return !keyboard.IsKeyPressed(ConvertKey(key));
  }

  public bool IsKeyPressed(Key key)
  {
    return keyboard.IsKeyPressed(ConvertKey(key));
  }

  public bool IsKeyReleased(Key key)
  {
    return !keyboard.IsKeyPressed(ConvertKey(key));
  }

  private static Silk.NET.Input.Key ConvertKey(Key key) => key switch
  {
    Key.F1 => Silk.NET.Input.Key.F1,
    Key.F2 => Silk.NET.Input.Key.F2,
    Key.F3 => Silk.NET.Input.Key.F3,
    Key.F4 => Silk.NET.Input.Key.F4,
    Key.F5 => Silk.NET.Input.Key.F5,
    Key.F6 => Silk.NET.Input.Key.F6,
    Key.F7 => Silk.NET.Input.Key.F7,
    Key.F8 => Silk.NET.Input.Key.F8,
    Key.F9 => Silk.NET.Input.Key.F9,
    Key.F10 => Silk.NET.Input.Key.F10,
    Key.F11 => Silk.NET.Input.Key.F11,
    Key.F12 => Silk.NET.Input.Key.F12,
    Key.W => Silk.NET.Input.Key.W,
    Key.S => Silk.NET.Input.Key.S,
    Key.A => Silk.NET.Input.Key.A,
    Key.D => Silk.NET.Input.Key.D,
    Key.Q => Silk.NET.Input.Key.Q,
    Key.E => Silk.NET.Input.Key.E,
    Key.R => Silk.NET.Input.Key.R,
    Key.Escape => Silk.NET.Input.Key.Escape,
    Key.Space => Silk.NET.Input.Key.Space,
    Key.Tilde => Silk.NET.Input.Key.GraveAccent,
    Key.LeftShift => Silk.NET.Input.Key.ShiftLeft,
    Key.Tab => Silk.NET.Input.Key.Tab,
    _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
  };
}
