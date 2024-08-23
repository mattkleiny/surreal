namespace Surreal.Memory;

/// <summary>
/// A nan-boxed value.
/// <para/>
/// This is a niche optimization that allows storing multiple kinds of values within a single 64-bit value,
/// and using properties of the 'NaN' state of a double to store the tag or kind of value that is contained.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal struct NanBox
{
  private const ulong MaskNan = 0x7FFC000000000000;
  private const ulong MaskBool = 0xFFE8000000000002;
  private const ulong MaskTrue = 0xFFF8000000000000;
  private const ulong MaskFalse = 0x7FF8000000000000;
  private const ulong MaskInt = 0x7FFC000000000000;
  private const ulong MaskPointer = 0xFFFC000000000000;
  private const ulong MaskNull = 0x7FFE000000000000;

  [FieldOffset(0)] public double Double;
  [FieldOffset(0)] public ulong Ulong;

  public static NanBox FromDouble(double value) => new() { Double = value };
  public static NanBox FromUlong(ulong value) => new() { Ulong = value };
  public static NanBox FromBool(bool value) => new() { Ulong = value ? MaskTrue : MaskFalse };
  public static NanBox FromInt(int value) => new() { Ulong = (uint)value | MaskInt };
  public static NanBox FromNull() => new() { Ulong = MaskNull };

  public readonly bool IsDouble => (Ulong & MaskNan) != MaskNan;
  public readonly bool IsInt    => (Ulong & MaskInt) == MaskInt;
  public readonly bool IsBool   => Ulong is MaskTrue or MaskFalse;
  public readonly bool IsTrue   => Ulong == MaskTrue;
  public readonly bool IsFalse  => Ulong == MaskFalse;
  public readonly bool IsNull   => Ulong == MaskNull;
}
