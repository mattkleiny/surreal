using Surreal.Input.Gamepad;
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
  /// All the attached <see cref="IInputDevice" />s.
  /// </summary>
  IEnumerable<IInputDevice> Devices { get; }

  /// <summary>
  /// An observable of all <see cref="IInputEvent" />s from all devices.
  /// </summary>
  IInputObservable Events { get; }

  /// <summary>
  /// A no-op <see cref="IInputBackend" /> for headless environments and testing.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullInputBackend : IInputBackend
  {
    public IEnumerable<IInputDevice> Devices { get; } =
    [
      IKeyboardDevice.Null,
      IMouseDevice.Null,
      IGamepadDevice.Null,
    ];

    public IInputObservable Events => IInputObservable.Null;
  }
}
