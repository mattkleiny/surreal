﻿using Silk.NET.Input;
using Surreal.Input.Keyboard;
using Key = Surreal.Input.Keyboard.Key;

namespace Surreal.Input;

/// <summary>
/// A <see cref="IKeyboardDevice"/> implementation that uses Silk.NET.
/// </summary>
internal sealed class SilkKeyboardDevice : IKeyboardDevice, IDisposable
{
  private readonly IKeyboard _keyboard;
  private readonly HashSet<Key> _pressedKeys = [];
  private readonly HashSet<Key> _pressedKeysThisFrame = [];
  private readonly InputEventSubject _events = new();

  public SilkKeyboardDevice(IKeyboard keyboard)
  {
    _keyboard = keyboard;

    keyboard.KeyDown += OnKeyDown;
    keyboard.KeyUp += OnKeyUp;
  }

  public event Action<Key>? KeyPressed;
  public event Action<Key>? KeyReleased;

  public IInputObservable Events => _events;

  public void Update()
  {
    foreach (var key in _pressedKeys)
    {
      _events.NotifyNext(new KeyHeldEvent(key));
    }

    _pressedKeysThisFrame.Clear();
  }

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
    return _pressedKeysThisFrame.Contains(key);
  }

  public void Dispose()
  {
    _keyboard.KeyDown -= OnKeyDown;
    _keyboard.KeyUp -= OnKeyUp;
  }

  private void OnKeyDown(IKeyboard keyboard, Silk.NET.Input.Key key, int index)
  {
    var keyCode = ConvertKey(key);
    if (keyCode != null)
    {
      if (_pressedKeys.Add(keyCode.Value))
      {
        _pressedKeysThisFrame.Add(keyCode.Value);

        KeyPressed?.Invoke(keyCode.Value);
        _events.NotifyNext(new KeyPressEvent(keyCode.Value, IsPressed: true));
      }
    }
  }

  private void OnKeyUp(IKeyboard keyboard, Silk.NET.Input.Key key, int index)
  {
    var keyCode = ConvertKey(key);
    if (keyCode != null)
    {
      if (_pressedKeys.Remove(keyCode.Value))
      {
        KeyReleased?.Invoke(keyCode.Value);
        _events.NotifyNext(new KeyPressEvent(keyCode.Value, IsPressed: false));
      }
    }
  }

  private static Key? ConvertKey(Silk.NET.Input.Key key) => key switch
  {
    Silk.NET.Input.Key.F1 => Key.F1,
    Silk.NET.Input.Key.F2 => Key.F2,
    Silk.NET.Input.Key.F3 => Key.F3,
    Silk.NET.Input.Key.F4 => Key.F4,
    Silk.NET.Input.Key.F5 => Key.F5,
    Silk.NET.Input.Key.F6 => Key.F6,
    Silk.NET.Input.Key.F7 => Key.F7,
    Silk.NET.Input.Key.F8 => Key.F8,
    Silk.NET.Input.Key.F9 => Key.F9,
    Silk.NET.Input.Key.F10 => Key.F10,
    Silk.NET.Input.Key.F11 => Key.F11,
    Silk.NET.Input.Key.F12 => Key.F12,
    Silk.NET.Input.Key.W => Key.W,
    Silk.NET.Input.Key.S => Key.S,
    Silk.NET.Input.Key.A => Key.A,
    Silk.NET.Input.Key.D => Key.D,
    Silk.NET.Input.Key.Q => Key.Q,
    Silk.NET.Input.Key.E => Key.E,
    Silk.NET.Input.Key.R => Key.R,
    Silk.NET.Input.Key.Escape => Key.Escape,
    Silk.NET.Input.Key.Space => Key.Space,
    Silk.NET.Input.Key.GraveAccent => Key.Tilde,
    Silk.NET.Input.Key.ShiftLeft => Key.LeftShift,
    Silk.NET.Input.Key.Tab => Key.Tab,
    _ => null
  };
}
