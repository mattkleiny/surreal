using System.Runtime.CompilerServices;

namespace Surreal.Serialization;

/// <summary>Allows writing values into a serialization stream.</summary>
public interface IBinaryWriter
{
  ValueTask WriteBoolAsync(bool value, CancellationToken cancellationToken = default)
  {
    Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<bool>()];
    BitConverter.TryWriteBytes(buffer, value);

    return WriteSpanAsync(buffer, cancellationToken);
  }

  ValueTask WriteByteAsync(byte value, CancellationToken cancellationToken = default)
  {
    Span<byte> buffer = stackalloc byte[] { value };

    return WriteSpanAsync(buffer, cancellationToken);
  }

  ValueTask WriteShortAsync(short value, CancellationToken cancellationToken = default)
  {
    Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<short>()];
    BitConverter.TryWriteBytes(buffer, value);

    return WriteSpanAsync(buffer, cancellationToken);
  }

  ValueTask WriteUShortAsync(ushort value, CancellationToken cancellationToken = default)
  {
    Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<ushort>()];
    BitConverter.TryWriteBytes(buffer, value);

    return WriteSpanAsync(buffer, cancellationToken);
  }

  ValueTask WriteIntAsync(int value, CancellationToken cancellationToken = default)
  {
    Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<int>()];
    BitConverter.TryWriteBytes(buffer, value);

    return WriteSpanAsync(buffer, cancellationToken);
  }

  ValueTask WriteUIntAsync(uint value, CancellationToken cancellationToken = default)
  {
    Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<uint>()];
    BitConverter.TryWriteBytes(buffer, value);

    return WriteSpanAsync(buffer, cancellationToken);
  }

  ValueTask WriteLongAsync(long value, CancellationToken cancellationToken = default)
  {
    Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<long>()];
    BitConverter.TryWriteBytes(buffer, value);

    return WriteSpanAsync(buffer, cancellationToken);
  }

  ValueTask WriteULongAsync(ulong value, CancellationToken cancellationToken = default)
  {
    Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<ulong>()];
    BitConverter.TryWriteBytes(buffer, value);

    return WriteSpanAsync(buffer, cancellationToken);
  }

  ValueTask WriteFloatAsync(float value, CancellationToken cancellationToken = default)
  {
    Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<float>()];
    BitConverter.TryWriteBytes(buffer, value);

    return WriteSpanAsync(buffer, cancellationToken);
  }

  ValueTask WriteDoubleAsync(double value, CancellationToken cancellationToken = default)
  {
    Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<double>()];
    BitConverter.TryWriteBytes(buffer, value);

    return WriteSpanAsync(buffer, cancellationToken);
  }

  ValueTask WriteSpanAsync(ReadOnlySpan<byte> value, CancellationToken cancellationToken = default);
  ValueTask WriteMemoryAsync(ReadOnlyMemory<byte> value, CancellationToken cancellationToken = default);
}
