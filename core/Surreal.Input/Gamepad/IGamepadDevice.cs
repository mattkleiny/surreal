namespace Surreal.Input.Gamepad;

/// <summary>
/// Represents a gamepad device.
/// </summary>
public interface IGamepadDevice : IInputDevice
{
  static IGamepadDevice Null { get; } = new NullGamepadDevice();

  Type IInputDevice.Type => typeof(IGamepadDevice);

  /// <summary>
  /// A no-op <see cref="IGamepadDevice"/> implementation.
  /// </summary>
  private sealed class NullGamepadDevice : IGamepadDevice;
}
