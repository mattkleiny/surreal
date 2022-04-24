using JetBrains.Annotations;

namespace Surreal.IO;

/// <summary>Associates a <see cref="BinarySerializer{T}"/> with it's source type.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public sealed class BinarySerializerAttribute : Attribute
{
  public BinarySerializerAttribute(Type type)
  {
    Type = type;
  }

  public Type Type { get; }
}

/// <summary>Abstracts over all possible <see cref="BinarySerializer{T}"/> types.</summary>
public abstract class BinarySerializer
{
  public static ValueTask SerializeAsync<T>(T value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public static ValueTask<T> DeserializeAsync<T>(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}

/// <summary>A serializer for reading/writing <see cref="T"/>s.</summary>
public abstract class BinarySerializer<T> : BinarySerializer
{
  public abstract ValueTask SerializeAsync(T value, IBinaryWriter writer, CancellationToken cancellationToken = default);
  public abstract ValueTask<T> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default);
}

/// <summary>Allows reading binary values from a serialization stream.</summary>
public interface IBinaryReader
{
  ValueTask<bool> ReadBoolAsync(CancellationToken cancellationToken = default);
  ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default);
  ValueTask<sbyte> ReadSByteAsync(CancellationToken cancellationToken = default);
  ValueTask<short> ReadShortAsync(CancellationToken cancellationToken = default);
  ValueTask<ushort> ReadUShortAsync(CancellationToken cancellationToken = default);
  ValueTask<int> ReadIntAsync(CancellationToken cancellationToken = default);
  ValueTask<uint> ReadUIntAsync(CancellationToken cancellationToken = default);
  ValueTask<long> ReadLongAsync(CancellationToken cancellationToken = default);
  ValueTask<ulong> ReadULongAsync(CancellationToken cancellationToken = default);
  ValueTask<float> ReadFloatAsync(CancellationToken cancellationToken = default);
  ValueTask<double> ReadDoubleAsync(CancellationToken cancellationToken = default);
  ValueTask<string> ReadStringAsync(CancellationToken cancellationToken = default);

  ValueTask ReadSpanAsync(Span<byte> buffer, CancellationToken cancellationToken = default);
  ValueTask ReadMemoryAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
}

/// <summary>Allows writing binary values to a serialization stream.</summary>
public interface IBinaryWriter
{
  ValueTask WriteBoolAsync(bool value, CancellationToken cancellationToken = default);
  ValueTask WriteByteAsync(byte value, CancellationToken cancellationToken = default);
  ValueTask WriteShortAsync(short value, CancellationToken cancellationToken = default);
  ValueTask WriteUShortAsync(ushort value, CancellationToken cancellationToken = default);
  ValueTask WriteIntAsync(int value, CancellationToken cancellationToken = default);
  ValueTask WriteUIntAsync(uint value, CancellationToken cancellationToken = default);
  ValueTask WriteLongAsync(long value, CancellationToken cancellationToken = default);
  ValueTask WriteULongAsync(ulong value, CancellationToken cancellationToken = default);
  ValueTask WriteFloatAsync(float value, CancellationToken cancellationToken = default);
  ValueTask WriteDoubleAsync(double value, CancellationToken cancellationToken = default);
  ValueTask WriteStringAsync(string value, CancellationToken cancellationToken = default);
  ValueTask WriteSpanAsync(ReadOnlySpan<byte> value, CancellationToken cancellationToken = default);
  ValueTask WriteMemoryAsync(ReadOnlyMemory<byte> value, CancellationToken cancellationToken = default);
}
