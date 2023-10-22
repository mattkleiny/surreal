namespace Surreal.Input;

/// <summary>
/// An input device on the platform.
/// </summary>
public interface IInputDevice
{
  /// <summary>
  /// The type of device.
  /// </summary>
  Type DeviceType { get; }
}
