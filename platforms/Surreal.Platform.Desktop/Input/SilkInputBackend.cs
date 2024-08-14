using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Surreal.Input;

internal sealed class SilkInputBackend(IWindow window, IInputContext context) : IInputBackend, IDisposable
{
  private readonly SilkKeyboardDevice _keyboardDevice = new(context.Keyboards.Single());
  private readonly SilkMouseDevice _mouseDevice = new(window, context.Mice.Single());

  /// <inheritdoc/>
  public IEnumerable<IInputDevice> DiscoverAllDevices()
  {
    return
    [
      _keyboardDevice,
      _mouseDevice
    ];
  }

  public void Update()
  {
    _keyboardDevice.Update();
  }

  public void Dispose()
  {
  }
}
