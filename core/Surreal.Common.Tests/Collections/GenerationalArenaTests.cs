using Surreal.Collections;

namespace Surreal.Common.Tests.Collections;

public class GenerationalArenaTests
{
  [Test]
  public void it_should_manage_a_simple_object_arena()
  {
    var arena = new GenerationalArena<Guid>();

    var id1 = arena.Add(Guid.NewGuid());
    var id2 = arena.Add(Guid.NewGuid());
    var id3 = arena.Add(Guid.NewGuid());

    arena.Remove(id2);

    var id4 = arena.Add(Guid.NewGuid());

    id4.Id.Should().Be(id2.Id);
    id4.Generation.Should().NotBe(id2.Generation);

    id1.Should().NotBe(id2);
    id1.Should().NotBe(id3);
  }
}
