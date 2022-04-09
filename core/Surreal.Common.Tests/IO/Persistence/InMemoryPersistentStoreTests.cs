using Surreal.Collections;
using Surreal.Persistence;

namespace Surreal.IO.Persistence;

public class InMemoryPersistentStoreTests
{
  private static Property<int>   Health    { get; } = new(nameof(Health));
  private static Property<float> Depth     { get; } = new(nameof(Depth));
  private static Property<bool>  IsEnabled { get; } = new(nameof(IsEnabled), true);

  [Test]
  public void it_should_persist_and_restore_properties()
  {
    var identifier = Guid.NewGuid();
    var store = new InMemoryPersistenceStore();

    store.CreateReader(identifier, out _).Should().BeFalse();

    var writer = store.CreateWriter(identifier);

    writer.Write(Health, 42);
    writer.Write(Depth, 25.05f);

    store.CreateReader(identifier, out var reader).Should().BeTrue();

    reader.Read(Health).Should().Be(42);
    reader.Read(Depth).Should().Be(25.05f);
    reader.Read(IsEnabled).Should().Be(true);
  }
}
