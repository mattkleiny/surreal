using Surreal.IO;

namespace Surreal.Scripting.Languages.Basic;

public class BasicParserTests
{
  [Test, Ignore("Not yet implemented")]
  [TestCase("Assets/scripts/basic/test01.bas")]
  public async Task it_should_parse_a_simple_program(VirtualPath path)
  {
    await using var stream = await path.OpenInputStreamAsync();
    IScriptParser   parser = new BasicScriptParser();

    var declaration = await parser.ParseScriptAsync(path.ToString(), stream, Encoding.UTF8);

    Assert.IsNotNull(declaration);
  }
}
