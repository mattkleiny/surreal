using Surreal.Collections;
using Surreal.Maths;

namespace Surreal.IO;

/// <summary>
/// A reader for binary data that wraps the .NET <see cref="System.IO.BinaryReader"/>.
/// </summary>
public sealed class FastBinaryReader(Stream stream, Encoding encoding) : IDisposable
{
  private readonly BinaryReader _reader = new(stream, encoding);

  public FastBinaryReader(Stream stream)
    : this(stream, Encoding.UTF8)
  {
  }

  /// <summary>
  /// The position of the reader in the stream.
  /// </summary>
  public long Position => _reader.BaseStream.Position;

  /// <summary>
  /// The length of the base stream.
  /// </summary>
  public long Length => _reader.BaseStream.Length;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte ReadByte() => _reader.ReadByte();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte[] ReadBytes(int count) => _reader.ReadBytes(count);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool ReadBool() => _reader.ReadBoolean();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int ReadInt16() => _reader.ReadInt16();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public uint ReadUInt16() => _reader.ReadUInt16();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int ReadInt32() => _reader.ReadInt32();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public uint ReadUInt32() => _reader.ReadUInt32();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public long ReadInt64() => _reader.ReadInt64();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ulong ReadUInt64() => _reader.ReadUInt64();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public float ReadFloat() => _reader.ReadSingle();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public double ReadDouble() => _reader.ReadDouble();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public string ReadString() => _reader.ReadString();

  public void Dispose()
  {
    _reader.Dispose();
  }
}

/// <summary>Commonly used extensions for <see cref="FastBinaryReader"/>.</summary>
public static class FastBinaryReaderExtensions
{
  public static Vector2 ReadVector2(this FastBinaryReader reader)
  {
    var x = reader.ReadFloat();
    var y = reader.ReadFloat();

    return new Vector2(x, y);
  }

  public static Point2 ReadPoint2(this FastBinaryReader reader)
  {
    var x = reader.ReadInt32();
    var y = reader.ReadInt32();

    return new Point2(x, y);
  }

  public static Vector3 ReadVector3(this FastBinaryReader reader)
  {
    var x = reader.ReadFloat();
    var y = reader.ReadFloat();
    var z = reader.ReadFloat();

    return new Vector3(x, y, z);
  }

  public static Point3 ReadPoint3(this FastBinaryReader reader)
  {
    var x = reader.ReadInt32();
    var y = reader.ReadInt32();
    var z = reader.ReadInt32();

    return new Point3(x, y, z);
  }

  public static Vector4 ReadVector4(this FastBinaryReader reader)
  {
    var x = reader.ReadFloat();
    var y = reader.ReadFloat();
    var z = reader.ReadFloat();
    var w = reader.ReadFloat();

    return new Vector4(x, y, z, w);
  }

  public static Point4 ReadPoint4(this FastBinaryReader reader)
  {
    var x = reader.ReadInt32();
    var y = reader.ReadInt32();
    var z = reader.ReadInt32();
    var w = reader.ReadInt32();

    return new Point4(x, y, z, w);
  }

  public static Quaternion ReadQuaternion(this FastBinaryReader reader)
  {
    var x = reader.ReadFloat();
    var y = reader.ReadFloat();
    var z = reader.ReadFloat();
    var w = reader.ReadFloat();

    return new Quaternion(x, y, z, w);
  }

  public static T ReadEnum<T>(this FastBinaryReader reader)
    where T : unmanaged, Enum
  {
    var ordinal = (int)reader.ReadByte();

    return ordinal.AsEnum<T>();
  }

  public static string? ReadNullableString(this FastBinaryReader reader)
  {
    if (reader.ReadBool())
    {
      return reader.ReadString();
    }

    return null;
  }

  public static void ReadCollection<T>(this FastBinaryReader reader, ICollection<T> collection)
    where T : IBinarySerializable<T>
  {
    collection.Clear();

    var count = reader.ReadInt32();

    for (var i = 0; i < count; i++)
    {
      collection.Add(reader.ReadSerializable<T>());
    }
  }

  public static T[] ReadArray<T>(this FastBinaryReader reader)
    where T : IBinarySerializable<T>
  {
    var count = reader.ReadInt32();
    var array = new T[count];

    for (var i = 0; i < count; i++)
    {
      array[i] = reader.ReadSerializable<T>();
    }

    return array;
  }

  public static Memory<T> ReadMemory<T>(this FastBinaryReader reader)
    where T : IBinarySerializable<T>
  {
    var count = reader.ReadInt32();
    var array = new T[count];

    for (var i = 0; i < count; i++)
    {
      array[i] = reader.ReadSerializable<T>();
    }

    return array;
  }

  public static List<T> ReadList<T>(this FastBinaryReader reader)
    where T : IBinarySerializable<T>
  {
    var collection = new List<T>();
    var count = reader.ReadInt32();

    for (var i = 0; i < count; i++)
    {
      collection.Add(reader.ReadSerializable<T>());
    }

    return collection;
  }

  public static HashSet<T> ReadHashSet<T>(this FastBinaryReader reader)
    where T : IBinarySerializable<T>
  {
    var collection = new HashSet<T>();
    var count = reader.ReadInt32();

    for (var i = 0; i < count; i++)
    {
      collection.Add(reader.ReadSerializable<T>());
    }

    return collection;
  }

  public static T ReadSerializable<T>(this FastBinaryReader reader)
    where T : IBinarySerializable<T>
  {
    return T.FromBinary(reader);
  }

  public static void ReadSpan<T>(this FastBinaryReader reader, Span<T> span)
    where T : IBinarySerializable<T>, new()
  {
    var array = new T[span.Length];

    for (var i = 0; i < span.Length; i++)
    {
      array[i] = reader.ReadSerializable<T>();
    }
  }
}
