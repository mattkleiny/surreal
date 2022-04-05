using Surreal.Assets;
using Surreal.IO.Json;

namespace Surreal.Objects;

public class JsonAssetLoaderTests
{
  [Test]
  public async Task it_should_import_a_simple_template_and_create_instance()
  {
    using var manager = new AssetManager();

    manager.AddLoader(new JsonAssetLoader());

    var template = await manager.LoadAssetAsync<TestTemplate>("Assets/templates/test.json");
    var instance = await manager.LoadAssetAsync<TestObject>("Assets/templates/test.json");

    template.Should().NotBeNull();
    template.Create().Should().NotBeNull();
    instance.Should().NotBeNull();
  }

  private sealed record TestObject;

  [Template(typeof(TestObject))]
  private sealed record TestTemplate : ITemplate<TestObject>
  {
    public string Name   { get; set; } = string.Empty;
    public int    Width  { get; set; }
    public int    Height { get; set; }

    public TestObject Create()
    {
      return new TestObject();
    }
  }
}
