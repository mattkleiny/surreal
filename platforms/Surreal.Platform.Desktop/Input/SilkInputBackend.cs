using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Surreal.Input;

/// <summary>
/// A <see cref="IInputBackend"/> implementation that uses Silk.NET.
/// </summary>
internal sealed class SilkInputBackend(IWindow window, IInputContext context) : IInputBackend
{
  private readonly SilkKeyboardDevice _keyboardDevice = new(context.Keyboards.Single());
  private readonly SilkMouseDevice _mouseDevice = new(window, context.Mice.Single());

  /// <inheritdoc/>
  public IEnumerable<IInputDevice> CreateDevices()
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
}
