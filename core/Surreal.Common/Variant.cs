using System.Runtime.InteropServices;
using Surreal.Colors;

namespace Surreal;

/// <summary>
/// Abstracts over a type that can be called.
/// </summary>
public delegate Variant Callable(params Variant[] args);

/// <summary>
/// Possible kinds of <see cref="Variant"/>.
/// </summary>
public enum VariantType : byte
{
  Null,
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
public struct Variant
{
  /// <summary>
  /// A null <see cref="Variant"/>.
  /// </summary>
  public static Variant Null => default;

  private VariantValue _value;
  private object? _object;

  /// <summary>
  /// The type of the <see cref="Variant"/>.
  /// </summary>
  public VariantType Type { get; }

  /// <summary>
  /// Creates a new <see cref="Variant"/> from a <see cref="object"/>.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Variant From(object input)
  {
    return input switch
    {
      bool value => value,
      byte value => value,
      short value => value,
      ushort value => value,
      int value => value,
      uint value => value,
      long value => value,
      ulong value => value,
      float value => value,
      double value => value,
      decimal value => value,
      Vector2 value => value,
      Vector3 value => value,
      Vector4 value => value,
      Quaternion value => value,
      Color value => value,
      Color32 value => value,
      string value => value,
      Array value => value,
      _ => new Variant(input)
    };
  }

  /// <summary>
  /// Creates a new <see cref="Variant"/> from a <see cref="object"/>.
  /// </summary>
  public static Variant From<T>(T value)
  {
    if (typeof(T) == typeof(bool)) return new Variant(Unsafe.As<T, bool>(ref value));
    if (typeof(T) == typeof(byte)) return new Variant(Unsafe.As<T, byte>(ref value));
    if (typeof(T) == typeof(short)) return new Variant(Unsafe.As<T, short>(ref value));
    if (typeof(T) == typeof(ushort)) return new Variant(Unsafe.As<T, ushort>(ref value));
    if (typeof(T) == typeof(int)) return new Variant(Unsafe.As<T, int>(ref value));
    if (typeof(T) == typeof(uint)) return new Variant(Unsafe.As<T, uint>(ref value));
    if (typeof(T) == typeof(long)) return new Variant(Unsafe.As<T, long>(ref value));
    if (typeof(T) == typeof(ulong)) return new Variant(Unsafe.As<T, ulong>(ref value));
    if (typeof(T) == typeof(float)) return new Variant(Unsafe.As<T, float>(ref value));
    if (typeof(T) == typeof(double)) return new Variant(Unsafe.As<T, double>(ref value));
    if (typeof(T) == typeof(decimal)) return new Variant(Unsafe.As<T, decimal>(ref value));
    if (typeof(T) == typeof(Vector2)) return new Variant(Unsafe.As<T, Vector2>(ref value));
    if (typeof(T) == typeof(Vector3)) return new Variant(Unsafe.As<T, Vector3>(ref value));
    if (typeof(T) == typeof(Vector4)) return new Variant(Unsafe.As<T, Vector4>(ref value));
    if (typeof(T) == typeof(Quaternion)) return new Variant(Unsafe.As<T, Quaternion>(ref value));
    if (typeof(T) == typeof(Color)) return new Variant(Unsafe.As<T, Color>(ref value));
    if (typeof(T) == typeof(Color32)) return new Variant(Unsafe.As<T, Color32>(ref value));
    if (typeof(T) == typeof(string)) return new Variant(Unsafe.As<T, string>(ref value));
    if (typeof(T) == typeof(Array)) return new Variant(Unsafe.As<T, Array>(ref value));

    return From(Unsafe.As<T, object>(ref value));
  }

  public override string ToString() => Type switch
  {
    VariantType.Null => "Variant (null)",
    VariantType.Bool => $"Variant (bool = {_value.Bool})",
    VariantType.Byte => $"Variant (byte = {_value.Byte})",
    VariantType.Short => $"Variant (short = {_value.Short})",
    VariantType.Ushort => $"Variant (ushort = {_value.Ushort})",
    VariantType.Int => $"Variant (int = {_value.Int})",
    VariantType.Uint => $"Variant (uint = {_value.Uint})",
    VariantType.Long => $"Variant (long = {_value.Long})",
    VariantType.Ulong => $"Variant (ulong = {_value.Ulong})",
    VariantType.Float => $"Variant (float = {_value.Float})",
    VariantType.Double => $"Variant (double = {_value.Double})",
    VariantType.Decimal => $"Variant (decimal = {_value.Decimal})",
    VariantType.Vector2 => $"Variant (Vector2 = {_value.Vector2})",
    VariantType.Vector3 => $"Variant (Vector3 = {_value.Vector3})",
    VariantType.Vector4 => $"Variant (Vector4 = {_value.Vector4})",
    VariantType.Quaternion => $"Variant (Quaternion = {_value.Quaternion})",
    VariantType.Color => $"Variant (Color = {_value.Color})",
    VariantType.Color32 => $"Variant (Color32 = {_value.Color32})",
    VariantType.String => $"Variant (string = {_object})",
    VariantType.Object => $"Variant (object = {_object})",
    VariantType.Array => $"Variant (Array = {_object})",

    _ => throw new ArgumentOutOfRangeException()
  };

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
  public T As<T>()
  {
    if (typeof(T) == typeof(bool)) return Unsafe.As<bool, T>(ref _value.Bool);
    if (typeof(T) == typeof(byte)) return Unsafe.As<byte, T>(ref _value.Byte);
    if (typeof(T) == typeof(short)) return Unsafe.As<short, T>(ref _value.Short);
    if (typeof(T) == typeof(ushort)) return Unsafe.As<ushort, T>(ref _value.Ushort);
    if (typeof(T) == typeof(int)) return Unsafe.As<int, T>(ref _value.Int);
    if (typeof(T) == typeof(uint)) return Unsafe.As<uint, T>(ref _value.Uint);
    if (typeof(T) == typeof(long)) return Unsafe.As<long, T>(ref _value.Long);
    if (typeof(T) == typeof(ulong)) return Unsafe.As<ulong, T>(ref _value.Ulong);
    if (typeof(T) == typeof(float)) return Unsafe.As<float, T>(ref _value.Float);
    if (typeof(T) == typeof(double)) return Unsafe.As<double, T>(ref _value.Double);
    if (typeof(T) == typeof(decimal)) return Unsafe.As<decimal, T>(ref _value.Decimal);
    if (typeof(T) == typeof(Vector2)) return Unsafe.As<Vector2, T>(ref _value.Vector2);
    if (typeof(T) == typeof(Vector3)) return Unsafe.As<Vector3, T>(ref _value.Vector3);
    if (typeof(T) == typeof(Vector4)) return Unsafe.As<Vector4, T>(ref _value.Vector4);
    if (typeof(T) == typeof(Quaternion)) return Unsafe.As<Quaternion, T>(ref _value.Quaternion);
    if (typeof(T) == typeof(Color)) return Unsafe.As<Color, T>(ref _value.Color);
    if (typeof(T) == typeof(Color32)) return Unsafe.As<Color32, T>(ref _value.Color32);
    if (_object != null) return Unsafe.As<object, T>(ref _object);

    return default!;
  }

  /// <summary>
  /// Converts the <see cref="Variant"/> to an <see cref="object"/>.
  /// </summary>
  public object? ToObject() => Type switch
  {
    VariantType.Null => null,
    VariantType.Bool => _value.Bool,
    VariantType.Byte => _value.Byte,
    VariantType.Short => _value.Short,
    VariantType.Ushort => _value.Ushort,
    VariantType.Int => _value.Int,
    VariantType.Uint => _value.Uint,
    VariantType.Long => _value.Long,
    VariantType.Ulong => _value.Ulong,
    VariantType.Float => _value.Float,
    VariantType.Double => _value.Double,
    VariantType.Decimal => _value.Decimal,
    VariantType.Vector2 => _value.Vector2,
    VariantType.Vector3 => _value.Vector3,
    VariantType.Vector4 => _value.Vector4,
    VariantType.Quaternion => _value.Quaternion,
    VariantType.Color => _value.Color,
    VariantType.Color32 => _value.Color32,
    VariantType.String => _object,
    VariantType.Object => _object,
    VariantType.Array => _object,
    _ => throw new ArgumentOutOfRangeException()
  };

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
