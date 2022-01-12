using Surreal.IO;

namespace Surreal.Scripting.Languages;

public class WrenScriptParserTests
{
  [Test, Ignore("Not yet implemented")]
  [TestCase("Assets/scripts/wren/test01.wren")]
  public async Task it_should_parse_wren_programs(VirtualPath path)
  {
    var parser      = new WrenScriptParser();
    var declaration = await parser.ParseScriptAsync(path);

    Assert.IsNotNull(declaration);
  }
}
