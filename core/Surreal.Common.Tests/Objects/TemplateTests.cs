﻿namespace Surreal.Objects;

public class TemplateTests
{
  [Test]
  public void it_should_create_a_valid_template_instance_from_factory()
  {
    TemplateFactory.Create<TestObject>().Should().NotBeNull();
  }

  private sealed class TestObject
  {
    [Template(typeof(TestObject))]
    private sealed record TestObjectBlueprint : ITemplate<TestObject>
    {
      public TestObject Create()
      {
        return new TestObject();
      }
    }
  }
}
