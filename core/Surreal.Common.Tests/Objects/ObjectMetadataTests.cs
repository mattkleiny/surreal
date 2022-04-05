namespace Surreal.Objects;

public class ObjectMetadataTests
{
  [Test]
  public void it_should_construct_from_arbitrary_object()
  {
    var instance = new TestObject();
    var metadata = ObjectMetadata.Create<TestObject>();

    metadata.Events.Should().NotBeEmpty();
    metadata.Methods.Should().NotBeEmpty();
    metadata.Properties.Should().NotBeEmpty();

    metadata.Methods[0].Invoke(instance);

    instance.Name.Should().Be("Test");
    instance.Age.Should().Be(2);
  }

  private sealed class TestObject
  {
    [Bind] public event Action<string>? NameChanged;
    [Bind] public event Action<int>?    AgeChanged;

    [Bind] public string Name { get; set; } = string.Empty;
    [Bind] public int    Age  { get; set; }

    [Bind] public void DoSomeWork()
    {
      Name =  "Test";
      Age  += 2;
    }
  }
}
