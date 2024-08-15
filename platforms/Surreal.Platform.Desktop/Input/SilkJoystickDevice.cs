using Silk.NET.Input;
using Surreal.Input.Joystick;

namespace Surreal.Input;

/// <summary>
/// A <see cref="IJoystickDevice"/> implementation that uses Silk.NET.
/// </summary>
internal sealed class SilkJoystickDevice(IJoystick joystick) : IJoystickDevice
{
  public Vector2 LeftSick => throw new NotImplementedException();
  public Vector2 RightSick => throw new NotImplementedException();
}
