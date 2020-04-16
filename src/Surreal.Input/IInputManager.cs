using System.Collections.Generic;
using System.Linq;

namespace Surreal.Input
{
  public interface IInputManager
  {
    IEnumerable<IInputDevice> Devices { get; }

    TDevice GetDevice<TDevice>()
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
}