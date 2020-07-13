using System;
using System.Collections.Generic;
using OpenTK.Input;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Key = Surreal.Input.Keyboard.Key;

namespace Surreal.Platform.Internal.Input {
  internal sealed class OpenTKKeyboardDevice : BufferedInputDevice<KeyboardState>, IKeyboardDevice {
    private readonly IDesktopWindow window;

    public OpenTKKeyboardDevice(IDesktopWindow window) {
      this.window = window;
    }

    public event Action<Key> KeyPressed  = null!;
    public event Action<Key> KeyReleased = null!;

    public bool IsKeyDown(Key key)     => CurrentState[Lookup[key]];
    public bool IsKeyUp(Key key)       => !CurrentState[Lookup[key]];
    public bool IsKeyPressed(Key key)  => CurrentState[Lookup[key]]  && !PreviousState[Lookup[key]];
    public bool IsKeyReleased(Key key) => PreviousState[Lookup[key]] && !CurrentState[Lookup[key]];

    public override void Update() {
      if (window.IsFocused) // only capture state if the window is focused
      {
        base.Update();

        // fire events, if necessary
        if (CurrentState != PreviousState) {
          foreach (var (key, _) in Lookup) {
            if (IsKeyPressed(key)) KeyPressed?.Invoke(key);
            if (IsKeyReleased(key)) KeyReleased?.Invoke(key);
          }
        }
      }
    }

    protected override KeyboardState CaptureState() => Keyboard.GetState();

    private static readonly Dictionary<Key, OpenTK.Input.Key> Lookup = new Dictionary<Key, OpenTK.Input.Key> {
        [Key.LeftShift] = OpenTK.Input.Key.ShiftLeft,
        [Key.F1]        = OpenTK.Input.Key.F1,
        [Key.F2]        = OpenTK.Input.Key.F2,
        [Key.F3]        = OpenTK.Input.Key.F3,
        [Key.F4]        = OpenTK.Input.Key.F4,
        [Key.F5]        = OpenTK.Input.Key.F5,
        [Key.F6]        = OpenTK.Input.Key.F6,
        [Key.F7]        = OpenTK.Input.Key.F7,
        [Key.F8]        = OpenTK.Input.Key.F8,
        [Key.F9]        = OpenTK.Input.Key.F9,
        [Key.F10]       = OpenTK.Input.Key.F10,
        [Key.F11]       = OpenTK.Input.Key.F11,
        [Key.F12]       = OpenTK.Input.Key.F12,
        [Key.W]         = OpenTK.Input.Key.W,
        [Key.S]         = OpenTK.Input.Key.S,
        [Key.A]         = OpenTK.Input.Key.A,
        [Key.D]         = OpenTK.Input.Key.D,
        [Key.Q]         = OpenTK.Input.Key.Q,
        [Key.E]         = OpenTK.Input.Key.E,
        [Key.Escape]    = OpenTK.Input.Key.Escape,
        [Key.Space]     = OpenTK.Input.Key.Space,
        [Key.Tilde]     = OpenTK.Input.Key.Tilde,
        [Key.Tab]       = OpenTK.Input.Key.Tab
    };
  }
}