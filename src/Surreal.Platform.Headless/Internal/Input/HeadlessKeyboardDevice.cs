using System;
using Surreal.Input.Keyboard;

namespace Surreal.Platform.Internal.Input {
  internal sealed class HeadlessKeyboardDevice : IHeadlessKeyboardDevice {
    public event Action<Key> KeyPressed;
    public event Action<Key> KeyReleased;

    public bool IsKeyDown(Key key)     => false;
    public bool IsKeyUp(Key key)       => false;
    public bool IsKeyPressed(Key key)  => false;
    public bool IsKeyReleased(Key key) => false;

    public void Update() {
    }
  }
}