using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Surreal.Input;

/// <summary>
/// A <see cref="IInputBackend"/> implementation that uses Silk.NET.
/// </summary>
internal sealed class SilkInputBackend : IInputBackend
{
  private readonly List<IInputDevice> _devices = new();

  /// <summary>
  /// A <see cref="IInputBackend"/> implementation that uses Silk.NET.
  /// </summary>
  public SilkInputBackend(IWindow window, IInputContext context)
  {
    _devices.AddRange(context.Keyboards.Select(keyboard => new SilkKeyboardDevice(keyboard)));
    _devices.AddRange(context.Mice.Select(mouse => new SilkMouseDevice(window, mouse)));
    _devices.AddRange(context.Gamepads.Select(gamepad => new SilkGamepadDevice(gamepad)));

    Events = IInputObservable.Combine(_devices.Select(it => it.Events));
  }

  /// <inheritdoc/>
  public IEnumerable<IInputDevice> Devices => _devices;

  /// <inheritdoc/>
  public IInputObservable Events { get; }

  public void Update()
  {
    foreach (var device in _devices)
    {
      if (device is SilkKeyboardDevice keyboardDevice)
      {
        keyboardDevice.Update();
      }
    }
  }
}
