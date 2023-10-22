using Silk.NET.Input;

namespace Surreal.Input;

internal sealed class SilkInputBackend(IInputContext context) : IInputBackend, IDisposable
{
  /// <inheritdoc/>
  public IEnumerable<IInputDevice> Devices { get; } = new IInputDevice[]
  {
    // TODO: make this more robust
    new SilkKeyboardDevice(context.Keyboards.Single()),
    new SilkMouseDevice(context.Mice.Single())
  };

  public void Dispose()
  {
  }
}
