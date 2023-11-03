using Surreal.Assets;
using Surreal.Scripting.Bytecode;
using Surreal.Scripting.Languages;
using Surreal.Utilities;

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

    registry.AddService<IScriptCompiler>(new BytecodeScriptCompiler());
    registry.AddService<IAssetLoader>(new ScriptLoader(new BasicScriptParser(), compiler));
    registry.AddService<IAssetLoader>(new ScriptLoader(new LispScriptParser(), compiler));
    registry.AddService<IAssetLoader>(new ScriptLoader(new LoxScriptParser(), compiler));
    registry.AddService<IAssetLoader>(new ScriptLoader(new LuaScriptParser(), compiler));
    registry.AddService<IAssetLoader>(new ScriptLoader(new WrenScriptParser(), compiler));
  }
}
