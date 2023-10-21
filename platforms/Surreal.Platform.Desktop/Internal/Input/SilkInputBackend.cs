namespace Surreal.Input;

internal sealed class SilkInputBackend : IInputBackend, IDisposable
{
  public IEnumerable<IInputDevice> Devices { get; } = Enumerable.Empty<IInputDevice>();

  public void Dispose()
  {
  }
}
