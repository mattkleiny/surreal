using Surreal.Utilities;

namespace Surreal.Scripting;

/// <summary>
/// A <see cref="IServiceModule"/> for the <see cref="Scripting"/> namespace.
/// </summary>
public sealed class ScriptingModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddAssemblyServices(Assembly.GetExecutingAssembly());
  }
}
