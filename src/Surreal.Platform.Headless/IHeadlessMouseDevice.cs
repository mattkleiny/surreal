using Surreal.Input.Mouse;
using Surreal.Mathematics.Linear;

namespace Surreal.Platform
{
  public interface IHeadlessMouseDevice : IMouseDevice
  {
    bool this[MouseButton button] { get; set; }
    new Point2 Position { get; set; }
  }
}