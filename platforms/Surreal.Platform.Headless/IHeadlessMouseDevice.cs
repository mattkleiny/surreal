using System.Numerics;
using Surreal.Input.Mouse;

namespace Surreal;

/// <summary>Allows access to the headless <see cref="IMouseDevice"/>.</summary>
public interface IHeadlessMouseDevice : IMouseDevice
{
  new Vector2 Position { get; set; }

  bool this[MouseButton button] { get; set; }
}
