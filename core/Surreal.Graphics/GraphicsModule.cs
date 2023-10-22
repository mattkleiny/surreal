using Surreal.Utilities;

namespace Surreal.Graphics;

/// <summary>
/// A <see cref="IServiceModule"/> for the <see cref="Graphics"/> namespace.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class GraphicsModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddAssemblyServices(Assembly.GetExecutingAssembly());
  }
}
