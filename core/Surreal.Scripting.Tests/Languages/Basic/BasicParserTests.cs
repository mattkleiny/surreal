using Surreal.IO;

namespace Surreal.Scripting.Languages.Basic;

public class BasicParserTests
{
  [Test, Ignore("Not yet implemented")]
  [TestCase("Assets/scripts/basic/test01.bas")]
  public async Task it_should_parse_basic_programs(VirtualPath path)
  {
    var parser      = new BasicScriptParser();
    var declaration = await parser.ParseScriptAsync(path);

    Assert.IsNotNull(declaration);
  }
}
