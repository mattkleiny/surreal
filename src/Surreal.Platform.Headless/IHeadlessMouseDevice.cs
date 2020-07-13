using Surreal.Input.Mouse;
using Surreal.Mathematics.Linear;

namespace Surreal.Platform {
  public interface IHeadlessMouseDevice : IMouseDevice {
    bool this[MouseButton button] { get; set; }
    new Vector2I Position { get; set; }
  }
}