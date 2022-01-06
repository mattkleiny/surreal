using Surreal.Utilities;

namespace Surreal.Components;

public class ComponentMaskTests
{
  [Test]
  public void it_should_create_a_valid_aspect()
  {
    var aspect = ComponentMask.Of<Transform>();

    Assert.IsTrue(aspect.Contains<Transform>());
    Assert.IsFalse(aspect.Contains<ComponentMaskTests>());
  }
}
