using Surreal.IO;
using Surreal.Scenes;

namespace Surreal.Framework.Tests.Scenes;

public class SceneTreeDefinitionTests
{
  [Test]
  [TestCase("Assets/External/scenes/test01.scene.json")]
  [TestCase("Assets/External/scenes/test01.scene.yml")]
  [TestCase("Assets/External/scenes/test01.scene.xml")]
  public async Task it_should_deserialize_scene_tree_from_disk(VirtualPath path)
  {
    var format = path.Extension switch
    {
      ".json" => FileFormat.Json,
      ".yml" => FileFormat.Yml,
      ".xml" => FileFormat.Xml,

      _ => throw new NotSupportedException($"Unsupported file extension: {path.Extension}")
    };

    var definition = await path.DeserializeAsync<SceneTreeDefinition>(format);

    definition.Name.Should().Be("Test Scene");
    definition.Description.Should().NotBeEmpty();
  }
}
