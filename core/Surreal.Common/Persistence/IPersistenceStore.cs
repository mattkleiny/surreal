using Surreal.Collections;

namespace Surreal.Persistence;

/// <summary>A store for persistent data.</summary>
public interface IPersistenceStore
{
  bool CreateReader(Guid identifier, out IPersistenceReader reader);
  IPersistenceWriter CreateWriter(Guid identifier);
}

/// <summary>A reader for persistent <see cref="Property{T}"/>s.</summary>
public interface IPersistenceReader
{
  T? Read<T>(Property<T> property, Optional<T> defaultValue = default);
}

/// <summary>A writer for persistent <see cref="Property{T}"/>s.</summary>
public interface IPersistenceWriter
{
  void Write<T>(Property<T> property, T value);
}
