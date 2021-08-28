using System;

namespace Surreal.Input
{
  public class DeviceNotFoundException : Exception
  {
    public DeviceNotFoundException(string message)
        : base(message)
    {
    }
  }
}
