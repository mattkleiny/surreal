using Silk.NET.Input;

namespace Surreal.Input;

internal sealed class SilkInputBackend(SilkWindow window) : IInputBackend, IDisposable
{
  public IEnumerable<IInputDevice> Devices { get; } = CreateInputDevices(window.Input);

  public void Dispose()
  {
  }

  private static List<IInputDevice> CreateInputDevices(IInputContext context)
  {
    var results = new List<IInputDevice>();

    results.AddRange(context.Keyboards.Select(keyboard => new SilkKeyboardDevice(keyboard)));
    results.AddRange(context.Mice.Select(mouse => new SilkMouseDevice(mouse)));

    return results;
  }
}
