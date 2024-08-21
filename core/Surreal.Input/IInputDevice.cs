namespace Surreal.Input;

/// <summary>
/// An input device on the platform.
/// </summary>
public interface IInputDevice
{
  /// <summary>
  /// The type of device.
  /// </summary>
  Type Type { get; }

  /// <summary>
  /// An observable of all <see cref="IInputEvent"/>s from this device.
  /// </summary>
  IInputObservable Events { get; }
}
