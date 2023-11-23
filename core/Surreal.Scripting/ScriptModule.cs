using Surreal.Assets;
using Surreal.Scripting.Languages;
using Surreal.Scripting.VirtualMachine;
using Surreal.Services;

namespace Surreal.Scripting;

/// <summary>
/// A module that provides scripting services.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class ScriptModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    var compiler = new BytecodeScriptCompiler();

    registry.AddService<IScriptCompiler>(compiler);

    registry.AddService<IScriptLanguage>(new BasicScriptLanguage());
    registry.AddService<IScriptLanguage>(new LispScriptLanguage());
    registry.AddService<IScriptLanguage>(new LoxScriptLanguage());
    registry.AddService<IScriptLanguage>(new LuaScriptLanguage());
    registry.AddService<IScriptLanguage>(new WrenScriptLanguage());

    registry.AddService<IAssetLoader>(new ScriptLoader(new BasicScriptParser(), compiler));
    registry.AddService<IAssetLoader>(new ScriptLoader(new LispScriptParser(), compiler));
    registry.AddService<IAssetLoader>(new ScriptLoader(new LoxScriptParser(), compiler));
    registry.AddService<IAssetLoader>(new ScriptLoader(new LuaScriptParser(), compiler));
    registry.AddService<IAssetLoader>(new ScriptLoader(new WrenScriptParser(), compiler));
  }
}
