using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;

namespace Surreal.Input;

/// <summary>
/// Manages <see cref="IInputDevice" />s.
/// </summary>
public interface IInputBackend
{
  /// <summary>
  /// A no-op <see cref="IInputBackend" /> for headless environments and testing.
  /// </summary>
  static IInputBackend Null { get; } = new NullInputBackend();

  /// <summary>
  /// Creates all the attached <see cref="IInputDevice" />s.
  /// </summary>
  IEnumerable<IInputDevice> CreateDevices();

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

    public IEnumerable<IInputDevice> CreateDevices() => _devices;
  }
}
