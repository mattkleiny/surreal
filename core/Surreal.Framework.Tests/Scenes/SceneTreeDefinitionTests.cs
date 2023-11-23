using Surreal.Assets;
using Surreal.IO;
using Surreal.Services;

namespace Surreal.Scenes;

public class SceneTreeDefinitionTests
{
  [Test]
  [TestCase("Assets/External/Scenes/test01.scene.json")]
  [TestCase("Assets/External/Scenes/test01.scene.yml")]
  [TestCase("Assets/External/Scenes/test01.scene.xml")]
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

  [Test]
  public void it_should_attach_scene_tree_as_new_children_to_existing_node()
  {
    using var tree = new SceneTree
    {
      Assets = new AssetManager(),
      Services = new ServiceRegistry()
    };

    var definition = new SceneTreeDefinition
    {
      Name = "A simple test tree",
      Description = "Testing direct hydration",
      Children =
      {
        new SceneNodeDefinition(),
        new SceneNodeDefinition(),
        new SceneNodeDefinition(),
      }
    };

    definition.BuildSubTree(tree);
  }
}
