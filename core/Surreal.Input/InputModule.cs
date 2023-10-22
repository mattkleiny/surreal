using Surreal.Utilities;

namespace Surreal.Input;

/// <summary>
/// A <see cref="IServiceModule"/> for the <see cref="Input"/> namespace.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class InputModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddAssemblyServices(Assembly.GetExecutingAssembly());
  }
}
