using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Surreal.Input;

internal sealed class SilkInputBackend(IWindow window, IInputContext context) : IInputBackend, IDisposable
{
  /// <inheritdoc/>
  public IEnumerable<IInputDevice> Devices { get; } = new IInputDevice[]
  {
    // TODO: make this more robust
    new SilkKeyboardDevice(context.Keyboards.Single()),
    new SilkMouseDevice(window, context.Mice.Single())
  };

  public void Dispose()
  {
  }
}
