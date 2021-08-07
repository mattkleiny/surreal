using System.Collections.Generic;
using Surreal.Input;

namespace Surreal.Platform.Internal.Input
{
  internal sealed class OpenTKInputManager : IInputManager
  {
    private readonly List<IInputDevice> devices = new();

    public OpenTKInputManager(OpenTKWindow window)
    {
      Keyboard = new OpenTKKeyboardDevice(window);
      Mouse    = new OpenTKMouseDevice(window);

      devices.Add(Keyboard);
      devices.Add(Mouse);
    }

    public OpenTKKeyboardDevice Keyboard { get; }
    public OpenTKMouseDevice    Mouse    { get; }

    public void Update()
    {
      Keyboard.Update();
      Mouse.Update();
    }

    public IEnumerable<IInputDevice> Devices => devices;
  }
}
