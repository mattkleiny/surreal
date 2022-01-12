using Surreal.IO;

namespace Surreal.Scripting.Languages;

public class LoxScriptParserTests
{
  [Test, Ignore("Not yet implemented")]
  [TestCase("Assets/scripts/lox/test01.lox")]
  public async Task it_should_parse_lox_programs(VirtualPath path)
  {
    var parser      = new LoxScriptParser();
    var declaration = await parser.ParseScriptAsync(path);

    Assert.IsNotNull(declaration);
  }
}
