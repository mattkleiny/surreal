using Surreal.Assets;
using Surreal.IO;
using Surreal.Scripting;

namespace Surreal;

public class LuaScriptServerTests
{
  [Test]
  [TestCase("Assets/scripts/test.lua")]
  public async Task it_should_execute_simple_scripts(VirtualPath path)
  {
    using var manager = new AssetManager();
    var server = new LuaScriptServer();

    manager.AddLoader(new ScriptLoader(server, ".lua"));

    var script = await manager.LoadAssetAsync<Script>(path);
    var result = script.ExecuteFunction("factorial", 4);

    result.Should().Be(24);
  }
}
