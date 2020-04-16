using Surreal.Input.Mouse;
using Surreal.Mathematics.Linear;

namespace Surreal.Platform
{
  public interface IHeadlessMouseDevice : IMouseDevice
  {
    new Vector2I Position { get; set; }
  }
}
