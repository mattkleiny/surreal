using Surreal.Assets;
using Surreal.IO;
using Surreal.Scripting;
using Surreal.Scripting.Lua;

namespace Surreal.Lua;

public class LuaScriptServerTests
{
  [Test]
  [TestCase("Assets/scripts/test.lua")]
  public async Task it_should_execute_simple_scripts(VirtualPath path)
  {
    using var manager = new AssetManager();
    var server = new LuaScriptServer();

    manager.AddLoader(new ScriptLoader(server));

    var script = await manager.LoadAsset<Script>(path);
    int result = script.ExecuteFunction("test");

    result.Should().Be(42);
  }
}
