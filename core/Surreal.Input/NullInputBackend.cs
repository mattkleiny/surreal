using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;

namespace Surreal.Input;

/// <summary>
/// A no-op <see cref="IInputBackend" /> for headless environments and testing.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class NullInputBackend : IInputBackend
{
  private readonly List<IInputDevice> _devices = [];

  public NullInputBackend()
  {
    _devices.Add(IKeyboardDevice.Null);
    _devices.Add(IMouseDevice.Null);
  }

  public IEnumerable<IInputDevice> DiscoverAllDevices() => _devices;
}
