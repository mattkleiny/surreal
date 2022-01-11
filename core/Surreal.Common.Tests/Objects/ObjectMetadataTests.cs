namespace Surreal.Objects;

public class ObjectMetadataTests
{
  [Test]
  public void it_should_construct_from_arbitrary_object()
  {
    var instance = new TestObject();
    var metadata = ObjectMetadata.Create<TestObject>();

    Assert.That(metadata.Events, Is.Not.Empty);
    Assert.That(metadata.Methods, Is.Not.Empty);
    Assert.That(metadata.Properties, Is.Not.Empty);

    metadata.Methods[0].Invoke(instance);

    Assert.AreEqual("Test", instance.Name);
    Assert.AreEqual(2, instance.Age);
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
