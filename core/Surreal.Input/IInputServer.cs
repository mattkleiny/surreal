namespace Surreal.Input;

/// <summary>Manages <see cref="IInputDevice" />s.</summary>
public interface IInputServer
{
  IEnumerable<IInputDevice> Devices { get; }

  bool HasDevice<TDevice>()
  {
    return Devices.OfType<TDevice>().Any();
  }

  TDevice? GetDevice<TDevice>()
    where TDevice : class, IInputDevice
  {
    return Devices.OfType<TDevice>().FirstOrDefault();
  }

  TDevice GetRequiredDevice<TDevice>()
    where TDevice : class, IInputDevice
  {
    var device = GetDevice<TDevice>();

    if (device == null)
    {
      throw new DeviceNotFoundException($"Unable to locate input device {typeof(TDevice)}");
    }

    return device;
  }

  IEnumerable<TDevice> GetDevices<TDevice>()
    where TDevice : class, IInputDevice
  {
    return Devices.OfType<TDevice>();
  }
}

/// <summary>Indicates an <see cref="IInputDevice" /> is not available.</summary>
public class DeviceNotFoundException : Exception
{
  public DeviceNotFoundException(string message)
    : base(message)
  {
  }
}



