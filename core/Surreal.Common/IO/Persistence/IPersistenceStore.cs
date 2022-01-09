namespace Surreal.IO.Persistence;

/// <summary>A store for persistent data.</summary>
public interface IPersistenceStore
{
  bool               CreateReader(Guid identifier, out IPersistenceReader reader);
  IPersistenceWriter CreateWriter(Guid identifier);
}

/// <summary>A reader for <see cref="PersistentProperty{T}"/>s.</summary>
public interface IPersistenceReader
{
  T? Read<T>(PersistentProperty<T> property, Optional<T> defaultValue = default);
}

/// <summary>A writer for <see cref="PersistentProperty{T}"/>s.</summary>
public interface IPersistenceWriter
{
  void Write<T>(PersistentProperty<T> property, T value);
}
