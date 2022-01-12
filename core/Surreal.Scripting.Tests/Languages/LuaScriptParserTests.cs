using Surreal.IO;

namespace Surreal.Scripting.Languages;

public class LuaScriptParserTests
{
  [Test, Ignore("Not yet implemented")]
  [TestCase("Assets/scripts/lua/test01.lua")]
  public async Task it_should_parse_lua_programs(VirtualPath path)
  {
    var parser      = new LuaScriptParser();
    var declaration = await parser.ParseScriptAsync(path);

    Assert.IsNotNull(declaration);
  }
}
