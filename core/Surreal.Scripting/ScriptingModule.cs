using Surreal.Scripting.Languages;
using Surreal.Services;

namespace Surreal.Scripting;

/// <summary>
/// A module that provides scripting services.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class ScriptingModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddService<IScriptLanguage>(new BasicScriptLanguage());
    registry.AddService<IScriptLanguage>(new LispScriptLanguage());
    registry.AddService<IScriptLanguage>(new LoxScriptLanguage());
    registry.AddService<IScriptLanguage>(new LuaScriptLanguage());
    registry.AddService<IScriptLanguage>(new WrenScriptLanguage());
  }
}
