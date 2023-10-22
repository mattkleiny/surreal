using Surreal.Utilities;

namespace Surreal.Audio;

/// <summary>
/// A <see cref="IServiceModule"/> for the <see cref="Audio"/> namespace.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class AudioModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddAssemblyServices(Assembly.GetExecutingAssembly());
  }
}
