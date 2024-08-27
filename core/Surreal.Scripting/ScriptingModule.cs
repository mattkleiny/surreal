using Surreal.Assets;
using Surreal.Scripting.Languages;
using Surreal.Scripting.Languages.Lox;
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
    registry.AddService<IScriptLanguage, LoxScriptLanguage>();

    registry.AddService<IAssetLoader, ScriptModuleLoader>();
  }
}
