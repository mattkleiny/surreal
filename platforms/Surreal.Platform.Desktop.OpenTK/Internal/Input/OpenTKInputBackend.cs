namespace Surreal.Input;

internal sealed class OpenTKInputBackend : IInputBackend
{
  private readonly List<IInputDevice> _devices = new();

  public OpenTKInputBackend(OpenTKWindow window)
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
