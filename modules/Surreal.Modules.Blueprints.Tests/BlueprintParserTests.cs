using Surreal.IO;

namespace Surreal;

public class BlueprintParserTests
{
  [Test]
  [TestCase("Assets/blueprints/common.blueprint")]
  [TestCase("Assets/blueprints/test01.blueprint")]
  public async Task it_should_parse_basic_blueprints(VirtualPath path)
  {
    var parser      = new BlueprintParser();
    var declaration = await parser.ParseAsync(path);

    declaration.Should().NotBeNull();
  }
}
