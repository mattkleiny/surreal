using Surreal.Input;

namespace Surreal.Internal.Input;

internal sealed class OpenTKInputServer : IInputServer
{
  private readonly List<IInputDevice> devices = new();

  public OpenTKInputServer(OpenTKWindow window)
  {
    Keyboard = new OpenTKKeyboardDevice(window);
    Mouse    = new OpenTKMouseDevice(window);

    devices.Add(Keyboard);
    devices.Add(Mouse);
  }

  public OpenTKKeyboardDevice Keyboard { get; }
  public OpenTKMouseDevice    Mouse    { get; }

  public IEnumerable<IInputDevice> Devices => devices;

  public void Update()
  {
    Keyboard.Update();
    Mouse.Update();
  }
}
