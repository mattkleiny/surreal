using Surreal.Input;

namespace Surreal.Internal.Input;

internal sealed class OpenTKInputServer : IInputServer
{
  private readonly List<IInputDevice> _devices = new();

  public OpenTKInputServer(OpenTKWindow window)
  {
    Keyboard = new OpenTKKeyboardDevice(window);
    Mouse = new OpenTKMouseDevice(window);

    _devices.Add(Keyboard);
    _devices.Add(Mouse);
  }

  public OpenTKKeyboardDevice Keyboard { get; }
  public OpenTKMouseDevice Mouse { get; }

  public IEnumerable<IInputDevice> Devices => _devices;

  public void Update()
  {
    Keyboard.Update();
    Mouse.Update();
  }
}


