using Surreal.Audio;
using Surreal.Graphics;
using Surreal.Input;
using Surreal.Scripting;
using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// A <see cref="IServiceModule"/> for the framework components.
/// </summary>
public sealed class FrameworkModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddAssemblyServices(Assembly.GetExecutingAssembly());

    registry.AddModule(new CommonModule());
    registry.AddModule(new AudioModule());
    registry.AddModule(new GraphicsModule());
    registry.AddModule(new InputModule());
    registry.AddModule(new ScriptingModule());
  }
}
