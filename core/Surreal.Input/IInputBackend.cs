namespace Surreal.Input;

/// <summary>
/// Manages <see cref="IInputDevice" />s.
/// </summary>
public interface IInputBackend
{
  IEnumerable<IInputDevice> Devices { get; }
}
