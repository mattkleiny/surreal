using System.Xml.Linq;
using Surreal.Assets;
using Surreal.IO.Xml;

namespace Surreal.Objects;

public class XmlAssetLoaderTests
{
  [Test]
  public async Task it_should_import_a_simple_template_and_create_instance()
  {
    using var manager = new AssetManager();

    manager.AddLoader(new XmlAssetLoader());

    var template = await manager.LoadAssetAsync<TestTemplate>("Assets/templates/test.xml");
    var instance = await manager.LoadAssetAsync<TestObject>("Assets/templates/test.xml");

    template.Should().NotBeNull();
    template.Create().Should().NotBeNull();
    instance.Should().NotBeNull();
  }

  private sealed record TestObject;

  [Template(typeof(TestObject))]
  private sealed record TestTemplate : ITemplate<TestObject>
  {
    public TestObject Create()
    {
      return new TestObject();
    }

    [XmlSerializer(typeof(TestTemplate))]
    private sealed class TemplateSerializer : XmlSerializer<TestTemplate>
    {
      public override ValueTask<TestTemplate> DeserializeAsync(XElement element, IXmlSerializationContext context, CancellationToken cancellationToken = default)
      {
        return new(new TestTemplate());
      }
    }
  }
}
