using Surreal.IO;

namespace Surreal;

public class BlueprintParserTests
{
  [Test]
  [TestCase("Assets/blueprints/test01.blueprint")]
  public async Task it_should_parse_basic_blueprints(VirtualPath path)
  {
    var parser      = new BlueprintParser();
    var declaration = await parser.ParseAsync(path);

    Assert.AreEqual(1, declaration.Includes.Length);
    Assert.AreEqual(7, declaration.Archetypes.Length);
    Assert.AreEqual(7, declaration.Archetypes[3].Components.Length);
    Assert.AreEqual(4, declaration.Archetypes[3].Events.Length);
  }
}
