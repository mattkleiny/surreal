namespace Surreal.Objects;

public class TemplateTests
{
  [Test]
  public void it_should_create_a_valid_template_instance_from_factory()
  {
    Assert.IsNotNull(TemplateFactory.Create<TestObject>());
  }

  private sealed class TestObject
  {
    [Template(typeof(TestObject))]
    private sealed class TestObjectBlueprint : ITemplate<TestObject>
    {
      public TestObject Create()
      {
        return new TestObject();
      }
    }
  }
}
