namespace Surreal.Input;

/// <summary>
/// Manages <see cref="IInputDevice" />s.
/// </summary>
public interface IInputServer
{
  IEnumerable<IInputDevice> Devices { get; }
}
