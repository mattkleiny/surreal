using System.Numerics;
using Surreal.Input.Mouse;

namespace Surreal.Platform
{
  public interface IHeadlessMouseDevice : IMouseDevice
  {
    bool this[MouseButton button] { get; set; }
    new Vector2 Position { get; set; }
  }
}
