using Surreal.Utilities;

namespace Surreal.Components;

public class ComponentMaskTests
{
  [Test]
  public void it_should_create_unique_masks()
  {
    var transform = ComponentMask.For<Transform>();
    var sprite = ComponentMask.For<Sprite>();
    var statistics = ComponentMask.For<Statistics>();

    transform.Should().NotBe(sprite);
    transform.Should().NotBe(statistics);
    sprite.Should().NotBe(statistics);
  }

  [Test]
  public void it_should_check_for_contains_all()
  {
    var transform = ComponentMask.For<Transform>();
    var sprite = ComponentMask.For<Sprite>();
    var statistics = ComponentMask.For<Statistics>();

    transform.ContainsAll(transform | sprite | statistics).Should().BeFalse();
    (transform | sprite).ContainsAll(transform | sprite | statistics).Should().BeFalse();
    (transform | sprite | statistics).ContainsAll(transform | sprite | statistics).Should().BeTrue();
  }

  [Test]
  public void it_should_check_for_contains_any()
  {
    var transform = ComponentMask.For<Transform>();
    var sprite = ComponentMask.For<Sprite>();
    var statistics = ComponentMask.For<Statistics>();

    ComponentMask.Empty.ContainsAny(transform | sprite | statistics).Should().BeFalse();

    transform.ContainsAny(transform | sprite | statistics).Should().BeTrue();
    (transform | sprite).ContainsAny(transform | sprite | statistics).Should().BeTrue();
    (transform | sprite | statistics).ContainsAny(transform | sprite | statistics).Should().BeTrue();
  }
}


