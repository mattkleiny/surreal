using System.Xml.Linq;
using Surreal.IO.Xml;

namespace Surreal.IO;

public class XmlSerializationTests
{
  [Test]
  public async Task it_should_deserialize_from_xml()
  {
    var element = XElement.Parse("<TestObject Id=\"1\" Name=\"Test\" />");
    var result = await XmlSerializer.DeserializeAsync<TestObject>(element);

    result.Should().NotBeNull();
    result.Id.Should().Be(1);
    result.Name.Should().Be("Test");
  }

  private sealed record TestObject(int Id, string Name);

  [XmlSerializer(typeof(TestObject))]
  private sealed class TestSerializer : XmlSerializer<TestObject>
  {
    public override ValueTask<TestObject> DeserializeAsync(XElement element, IXmlSerializationContext context, CancellationToken cancellationToken = default)
    {
      var result = new TestObject(
        (int) element.Attribute("Id")!,
        (string) element.Attribute("Name")!
      );

      return new(result);
    }
  }
}
