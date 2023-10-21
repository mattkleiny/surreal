using Surreal.Collections;
using Surreal.Maths;

namespace Surreal.IO;

/// <summary>
/// A reader for binary data that wraps the .NET <see cref="System.IO.BinaryWriter"/>.
/// </summary>
public sealed class FastBinaryWriter(Stream stream, Encoding encoding) : IDisposable, IAsyncDisposable
{
  private readonly BinaryWriter _writer = new(stream, encoding);

  public FastBinaryWriter(Stream stream)
    : this(stream, Encoding.UTF8)
  {
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void WriteByte(byte value) => _writer.Write(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void WriteBytes(byte[] value) => _writer.Write(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void WriteBool(bool value) => _writer.Write(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void WriteInt16(short value) => _writer.Write(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void WriteUInt16(ushort value) => _writer.Write(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void WriteInt32(int value) => _writer.Write(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void WriteUInt32(uint value) => _writer.Write(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void WriteInt64(long value) => _writer.Write(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void WriteUInt64(ulong value) => _writer.Write(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void WriteFloat(float value) => _writer.Write(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void WriteDouble(double value) => _writer.Write(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void WriteString(string value) => _writer.Write(value);

  public void Dispose()
  {
    _writer.Dispose();
  }

  public ValueTask DisposeAsync()
  {
    return _writer.DisposeAsync();
  }
}

/// <summary>
/// Commonly used extensions for <see cref="FastBinaryWriter"/>.
/// </summary>
public static class FastBinaryWriterExtensions
{
  public static void WriteVector2(this FastBinaryWriter writer, Vector2 vector)
  {
    writer.WriteFloat(vector.X);
    writer.WriteFloat(vector.Y);
  }

  public static void WritePoint2(this FastBinaryWriter writer, Point2 point)
  {
    writer.WriteInt32(point.X);
    writer.WriteInt32(point.Y);
  }

  public static void WriteVector3(this FastBinaryWriter writer, Vector3 vector)
  {
    writer.WriteFloat(vector.X);
    writer.WriteFloat(vector.Y);
    writer.WriteFloat(vector.Z);
  }

  public static void WritePoint3(this FastBinaryWriter writer, Point3 point)
  {
    writer.WriteInt32(point.X);
    writer.WriteInt32(point.Y);
    writer.WriteInt32(point.Z);
  }

  public static void WriteVector4(this FastBinaryWriter writer, Vector4 vector)
  {
    writer.WriteFloat(vector.X);
    writer.WriteFloat(vector.Y);
    writer.WriteFloat(vector.Z);
    writer.WriteFloat(vector.W);
  }

  public static void WritePoint4(this FastBinaryWriter writer, Point4 point)
  {
    writer.WriteFloat(point.X);
    writer.WriteFloat(point.Y);
    writer.WriteFloat(point.Z);
    writer.WriteFloat(point.W);
  }

  public static void WriteQuaternion(this FastBinaryWriter writer, Quaternion quaternion)
  {
    writer.WriteFloat(quaternion.X);
    writer.WriteFloat(quaternion.Y);
    writer.WriteFloat(quaternion.Z);
    writer.WriteFloat(quaternion.W);
  }

  public static void WriteEnum<T>(this FastBinaryWriter writer, T value)
    where T : unmanaged, Enum
  {
    var ordinal = (byte)value.AsInt();

    writer.WriteByte(ordinal);
  }

  public static void WriteNullableString(this FastBinaryWriter writer, string? value)
  {
    if (value != null)
    {
      writer.WriteBool(true);
      writer.WriteString(value);
    }
    else
    {
      writer.WriteBool(false);
    }
  }

  public static void WriteSlice<T>(this FastBinaryWriter writer, ReadOnlySlice<T> collection)
    where T : IBinarySerializable
  {
    writer.WriteInt32(collection.Length);

    foreach (var item in collection)
    {
      writer.WriteSerializable(item);
    }
  }

  public static void WriteSpan<T>(this FastBinaryWriter writer, ReadOnlySpan<T> collection)
    where T : IBinarySerializable
  {
    writer.WriteInt32(collection.Length);

    foreach (var item in collection)
    {
      writer.WriteSerializable(item);
    }
  }

  public static void WriteList<T>(this FastBinaryWriter writer, IReadOnlyList<T> list)
    where T : IBinarySerializable
  {
    writer.WriteInt32(list.Count);

    for (var i = 0; i < list.Count; i++)
    {
      writer.WriteSerializable(list[i]);
    }
  }

  public static void WriteCollection<T>(this FastBinaryWriter writer, ICollection<T> collection)
    where T : IBinarySerializable
  {
    writer.WriteInt32(collection.Count);

    foreach (var item in collection)
    {
      writer.WriteSerializable(item);
    }
  }

  public static void WriteSerializable<T>(this FastBinaryWriter writer, T serializable)
    where T : IBinarySerializable
  {
    serializable.Save(writer);
  }
}
