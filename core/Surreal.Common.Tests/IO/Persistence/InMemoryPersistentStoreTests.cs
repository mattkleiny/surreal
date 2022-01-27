using Surreal.Collections;

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
    var store      = new InMemoryPersistenceStore();

    Assert.IsFalse(store.CreateReader(identifier, out _));

    var writer = store.CreateWriter(identifier);

    writer.Write(Health, 42);
    writer.Write(Depth, 25.05f);

    Assert.IsTrue(store.CreateReader(identifier, out var reader));

    Assert.AreEqual(42, reader.Read(Health));
    Assert.AreEqual(25.05f, reader.Read(Depth));
    Assert.IsTrue(reader.Read(IsEnabled)); // should use default values
  }
}
