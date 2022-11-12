using Surreal.Actors.Components;
using Surreal.Actors.Utilities;

namespace Surreal.Actors.Aspects;

public class AspectTests
{
  [Test]
  public void it_should_create_a_valid_aspect()
  {
    var transform = ComponentMask.For<Transform>();
    var sprite = ComponentMask.For<Sprite>();
    var statistics = ComponentMask.For<Statistics>();

    var aspect = new Aspect()
      .With<Transform>()
      .With<Sprite>()
      .Without<Statistics>();

    aspect.IsInterestedIn(transform).Should().BeFalse();
    aspect.IsInterestedIn(transform | sprite).Should().BeTrue();
    aspect.IsInterestedIn(transform | sprite | statistics).Should().BeFalse();
  }
}



