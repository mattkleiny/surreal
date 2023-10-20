namespace Surreal.Input;

/// <summary>
/// Indicates an <see cref="IInputDevice" /> is not available.
/// </summary>
public class DeviceNotFoundException(string message) : ApplicationException(message);
