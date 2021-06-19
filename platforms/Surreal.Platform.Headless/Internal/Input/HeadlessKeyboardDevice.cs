using System;
using System.Collections.Generic;
using Surreal.Input.Keyboard;

namespace Surreal.Platform.Internal.Input
{
  internal sealed class HeadlessKeyboardDevice : IHeadlessKeyboardDevice
  {
    private readonly HashSet<Key> pressedKeys = new();

    public event Action<Key> KeyPressed  = null!;
    public event Action<Key> KeyReleased = null!;

    public bool IsKeyDown(Key key)     => pressedKeys.Contains(key);
    public bool IsKeyUp(Key key)       => !pressedKeys.Contains(key);
    public bool IsKeyPressed(Key key)  => pressedKeys.Contains(key);
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
}