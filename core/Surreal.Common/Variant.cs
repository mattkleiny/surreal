using System.Runtime.InteropServices;
using Surreal.Colors;
using Color = System.Drawing.Color;

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
/// A variant type that can hold any type of common data.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly struct Variant
{
  [FieldOffset(0)] private readonly VariantType _type;
  [FieldOffset(1)] private readonly bool _bool;
  [FieldOffset(1)] private readonly byte _byte;
  [FieldOffset(1)] private readonly short _short;
  [FieldOffset(1)] private readonly ushort _ushort;
  [FieldOffset(1)] private readonly int _int;
  [FieldOffset(1)] private readonly uint _uint;
  [FieldOffset(1)] private readonly long _long;
  [FieldOffset(1)] private readonly ulong _ulong;
  [FieldOffset(1)] private readonly float _float;
  [FieldOffset(1)] private readonly double _double;
  [FieldOffset(1)] private readonly decimal _decimal;
  [FieldOffset(1)] private readonly Vector2 _vector2;
  [FieldOffset(1)] private readonly Vector3 _vector3;
  [FieldOffset(1)] private readonly Vector4 _vector4;
  [FieldOffset(1)] private readonly Quaternion _quaternion;
  [FieldOffset(1)] private readonly Color _color;
  [FieldOffset(1)] private readonly Color32 _color32;
  [FieldOffset(1)] private readonly object? _object;
  [FieldOffset(1)] private readonly Array? _array;

  /// <summary>
  /// Creates a new <see cref="Variant"/> from a <see cref="object"/>.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Variant From(object value)
  {
    return new Variant(value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(bool value)
  {
    _type = VariantType.Bool;
    _bool = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(byte value)
  {
    _type = VariantType.Byte;
    _byte = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(short value)
  {
    _type = VariantType.Short;
    _short = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(ushort value)
  {
    _type = VariantType.Ushort;
    _ushort = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(int value)
  {
    _type = VariantType.Int;
    _int = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(uint value)
  {
    _type = VariantType.Uint;
    _uint = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(long value)
  {
    _type = VariantType.Long;
    _long = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(ulong value)
  {
    _type = VariantType.Ulong;
    _ulong = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(float value)
  {
    _type = VariantType.Float;
    _float = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(double value)
  {
    _type = VariantType.Double;
    _double = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(decimal value)
  {
    _type = VariantType.Decimal;
    _decimal = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Vector2 value)
  {
    _type = VariantType.Vector2;
    _vector2 = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Vector3 value)
  {
    _type = VariantType.Vector3;
    _vector3 = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Vector4 value)
  {
    _type = VariantType.Vector4;
    _vector4 = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Quaternion value)
  {
    _type = VariantType.Quaternion;
    _quaternion = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Color value)
  {
    _type = VariantType.Color;
    _color = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Color32 value)
  {
    _type = VariantType.Color32;
    _color32 = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(string value)
  {
    _type = VariantType.String;
    _object = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(object value)
  {
    _type = VariantType.Object;
    _object = value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private Variant(Array value)
  {
    _type = VariantType.Array;
    _array = value;
  }

  public VariantType Type => _type;

  // explicit conversion
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool AsBool() => _bool;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte AsByte() => _byte;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public short AsShort() => _short;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ushort AsUshort() => _ushort;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int AsInt() => _int;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public uint AsUint() => _uint;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public long AsLong() => _long;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ulong AsUlong() => _ulong;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public float AsFloat() => _float;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public double AsDouble() => _double;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public decimal AsDecimal() => _decimal;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector2 AsVector2() => _vector2;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector3 AsVector3() => _vector3;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector4 AsVector4() => _vector4;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Quaternion AsQuaternion() => _quaternion;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Color AsColor() => _color;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Color32 AsColor32() => _color32;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public string? AsString() => _object as string;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public object? AsObject() => _object;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Array? AsArray() => _array;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public T? As<T>() where T : class => _object as T;

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
}
