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
    registry.AddService(new BytecodeVirtualMachine());

    registry.AddService<IAssetLoader>(new ScriptLoader(new BytecodeScriptCompiler()));
    registry.AddService<IAssetLoader>(new ScriptDeclarationLoader(new BasicScriptParser()));
    registry.AddService<IAssetLoader>(new ScriptDeclarationLoader(new LispScriptParser()));
    registry.AddService<IAssetLoader>(new ScriptDeclarationLoader(new LoxScriptParser()));
    registry.AddService<IAssetLoader>(new ScriptDeclarationLoader(new LuaScriptParser()));
    registry.AddService<IAssetLoader>(new ScriptDeclarationLoader(new WrenScriptParser()));
  }
}
