using Surreal.Input;

namespace Surreal.Internal.Input;

internal sealed class SilkInputBackend : IInputBackend
{
  public IEnumerable<IInputDevice> Devices => throw new NotImplementedException();
}
