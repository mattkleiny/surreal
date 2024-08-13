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
  IEnumerable<IInputDevice> DiscoverAllDevices();
}
