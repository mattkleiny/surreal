using Surreal.IO;

namespace Surreal.Scripting.Languages;

public class LispScriptParserTests
{
  [Test, Ignore("Not yet implemented")]
  [TestCase("Assets/scripts/lisp/test01.lsp")]
  public async Task it_should_parse_lisp_programs(VirtualPath path)
  {
    var parser      = new LispScriptParser();
    var declaration = await parser.ParseScriptAsync(path);

    declaration.Should().NotBeNull();
  }
}
