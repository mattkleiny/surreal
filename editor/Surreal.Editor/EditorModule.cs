using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// A <see cref="IServiceModule"/> for the editor.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class EditorModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddAssemblyServices(Assembly.GetExecutingAssembly());
  }
}
