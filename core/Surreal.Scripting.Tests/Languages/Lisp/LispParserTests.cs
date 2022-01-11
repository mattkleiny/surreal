using Surreal.IO;

namespace Surreal.Scripting.Languages.Lisp;

public class LispParserTests
{
  [Test, Ignore("Not yet implemented")]
  [TestCase("Assets/scripts/lisp/test01.lsp")]
  public async Task it_should_parse_lisp_programs(VirtualPath path)
  {
    var parser      = new LispScriptParser();
    var declaration = await parser.ParseScriptAsync(path);

    Assert.IsNotNull(declaration);
  }
}
