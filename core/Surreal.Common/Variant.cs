using System.Runtime.InteropServices;
using Surreal.Colors;

namespace Surreal;

/// <summary>
/// Possible kinds of <see cref="Variant"/>.
/// </summary>
public enum VariantType : byte
{
  Bool,
  Byte,
  Short,
  Ushort,
  Int,
  Uint,
  Long,
  Ulong,
  Float,
  Double,
  Decimal,
  Vector2,
  Vector3,
  Vector4,
  Quaternion,
  Color,
  Color32,
  String,
  Object,
  Array
}

/// <summary>
/// A union of all possible <see cref="Variant"/> primitive values.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal struct VariantValue
{
  [FieldOffset(0)] public bool Bool;
  [FieldOffset(0)] public byte Byte;
  [FieldOffset(0)] public short Short;
  [FieldOffset(0)] public ushort Ushort;
  [FieldOffset(0)] public int Int;
  [FieldOffset(0)] public uint Uint;
  [FieldOffset(0)] public long Long;
  [FieldOffset(0)] public ulong Ulong;
  [FieldOffset(0)] public float Float;
  [FieldOffset(0)] public double Double;
  [FieldOffset(0)] public decimal Decimal;
  [FieldOffset(0)] public Vector2 Vector2;
  [FieldOffset(0)] public Vector3 Vector3;
  [FieldOffset(0)] public Vector4 Vector4;
  [FieldOffset(0)] public Quaternion Quaternion;
  [FieldOffset(0)] public Color Color;
  [FieldOffset(0)] public Color32 Color32;
}

/// <summary>
/// A variant type that can hold any type of common data in at most 16 bytes.
/// </summary>
public readonly struct Variant
{
  private readonly VariantValue _value;
  private readonly object? _object;

  /// <summary>
  /// Creates a new <see cref="Variant"/> from a <see cref="object"/>.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Variant From(object value) => new(value);

  /// <summary>
  /// The type of the <see cref="Variant"/>.
  /// </summary>
  public VariantType Type { get; }

  #region Constructors

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(bool value)
  {
    Type = VariantType.Bool;
    _value.Bool = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(byte value)
  {
    Type = VariantType.Byte;
    _value.Byte = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(short value)
  {
    Type = VariantType.Short;
    _value.Short = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(ushort value)
  {
    Type = VariantType.Ushort;
    _value.Ushort = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(int value)
  {
    Type = VariantType.Int;
    _value.Int = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(uint value)
  {
    Type = VariantType.Uint;
    _value.Uint = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(long value)
  {
    Type = VariantType.Long;
    _value.Long = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(ulong value)
  {
    Type = VariantType.Ulong;
    _value.Ulong = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(float value)
  {
    Type = VariantType.Float;
    _value.Float = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(double value)
  {
    Type = VariantType.Double;
    _value.Double = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(decimal value)
  {
    Type = VariantType.Decimal;
    _value.Decimal = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Vector2 value)
  {
    Type = VariantType.Vector2;
    _value.Vector2 = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Vector3 value)
  {
    Type = VariantType.Vector3;
    _value.Vector3 = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Vector4 value)
  {
    Type = VariantType.Vector4;
    _value.Vector4 = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Quaternion value)
  {
    Type = VariantType.Quaternion;
    _value.Quaternion = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Color value)
  {
    Type = VariantType.Color;
    _value.Color = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Color32 value)
  {
    Type = VariantType.Color32;
    _value.Color32 = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(string value)
  {
    Type = VariantType.String;
    _object = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(object value)
  {
    Type = VariantType.Object;
    _object = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Array value)
  {
    Type = VariantType.Array;
    _object = value;
  }

  #endregion

  #region Explicit Conversions

  // explicit conversion
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool AsBool() => _value.Bool;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte AsByte() => _value.Byte;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public short AsShort() => _value.Short;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ushort AsUshort() => _value.Ushort;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int AsInt() => _value.Int;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public uint AsUint() => _value.Uint;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public long AsLong() => _value.Long;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ulong AsUlong() => _value.Ulong;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public float AsFloat() => _value.Float;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public double AsDouble() => _value.Double;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public decimal AsDecimal() => _value.Decimal;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector2 AsVector2() => _value.Vector2;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector3 AsVector3() => _value.Vector3;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector4 AsVector4() => _value.Vector4;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Quaternion AsQuaternion() => _value.Quaternion;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Color AsColor() => _value.Color;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Color32 AsColor32() => _value.Color32;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public string? AsString() => _object as string;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public object? AsObject() => _object;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Array? AsArray() => _object as Array;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public T? As<T>()
    where T : class => _object as T;

  #endregion

  #region Implicit Conversions

  // implicit conversion
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(bool value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(byte value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(short value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(ushort value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(int value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(uint value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(long value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(ulong value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(float value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(double value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(decimal value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(Vector2 value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(Vector3 value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(Vector4 value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(Quaternion value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(Color value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(Color32 value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(string value) => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Variant(Array value) => new(value);

  #endregion
}
