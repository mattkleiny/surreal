using OpenTK.Windowing.GraphicsLibraryFramework;
using Surreal.Input.Keyboard;

namespace Surreal.Internal.Input;

internal sealed class OpenTKKeyboardDevice : IKeyboardDevice
{
  private readonly OpenTKWindow  window;
  private readonly KeyboardState keyboardState;

  public OpenTKKeyboardDevice(OpenTKWindow window)
  {
    this.window = window;

    keyboardState = window.KeyboardState;
  }

  public event Action<Key> KeyPressed  = null!;
  public event Action<Key> KeyReleased = null!;

  public bool IsKeyDown(Key key)     => keyboardState.IsKeyDown(Lookup[key]);
  public bool IsKeyUp(Key key)       => !keyboardState.IsKeyDown(Lookup[key]);
  public bool IsKeyPressed(Key key)  => keyboardState.IsKeyPressed(Lookup[key]);
  public bool IsKeyReleased(Key key) => keyboardState.IsKeyReleased(Lookup[key]);

  public void Update()
  {
    // only capture state if the window is focused
    if (window.IsFocused)
    {
      foreach (var (key, _) in Lookup)
      {
        if (IsKeyPressed(key)) KeyPressed?.Invoke(key);
        if (IsKeyReleased(key)) KeyReleased?.Invoke(key);
      }
    }
  }

  private static readonly Dictionary<Key, Keys> Lookup = new()
  {
    [Key.LeftShift] = Keys.LeftShift,
    [Key.F1]        = Keys.F1,
    [Key.F2]        = Keys.F2,
    [Key.F3]        = Keys.F3,
    [Key.F4]        = Keys.F4,
    [Key.F5]        = Keys.F5,
    [Key.F6]        = Keys.F6,
    [Key.F7]        = Keys.F7,
    [Key.F8]        = Keys.F8,
    [Key.F9]        = Keys.F9,
    [Key.F10]       = Keys.F10,
    [Key.F11]       = Keys.F11,
    [Key.F12]       = Keys.F12,
    [Key.W]         = Keys.W,
    [Key.S]         = Keys.S,
    [Key.A]         = Keys.A,
    [Key.D]         = Keys.D,
    [Key.Q]         = Keys.Q,
    [Key.E]         = Keys.E,
    [Key.Escape]    = Keys.Escape,
    [Key.Space]     = Keys.Space,
    [Key.Tilde]     = Keys.GraveAccent,
    [Key.Tab]       = Keys.Tab,
  };
}