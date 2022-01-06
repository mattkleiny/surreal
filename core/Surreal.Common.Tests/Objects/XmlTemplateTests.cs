using Surreal.Assets;

namespace Surreal.Objects;

public class XmlTemplateTests
{
  [Test]
  public async Task it_should_import_a_simple_template()
  {
    using var manager = new AssetManager();

    manager.AddLoader(new XmlTemplateLoader());

    var template = await manager.LoadAsset<TestTemplate>("Assets/templates/test.xml");

    Assert.IsNotNull(template);
    Assert.IsNotNull(template.Create());
  }

  private sealed class TestTemplate : IImportableTemplate<object>
  {
    public object Create()
    {
      return new object();
    }

    public void OnImportTemplate(ITemplateImportContext context)
    {
    }
  }
}
