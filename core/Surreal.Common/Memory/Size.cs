using System.Runtime.CompilerServices;

namespace Surreal.Memory;

/// <summary>Represents a measure of bytes, convertible to other representations.</summary>
public readonly record struct Size(long Bytes) : IComparable<Size>, IComparable
{
  public static readonly Size Zero = new(0);

  public float Kilobytes => Bytes / 1024f;
  public float Megabytes => Kilobytes / 1024f;
  public float Gigabytes => Megabytes / 1024f;
  public float Terabytes => Gigabytes / 1024f;

  public override string ToString()
  {
    if (Terabytes > 1) return $"{Terabytes:F} terabytes";
    if (Gigabytes > 1) return $"{Gigabytes:F} gigabytes";
    if (Megabytes > 1) return $"{Megabytes:F} megabytes";
    if (Kilobytes > 1) return $"{Kilobytes:F} kilobytes";

    return $"{Bytes} bytes";
  }

  public int CompareTo(Size other)
  {
    return Bytes.CompareTo(other.Bytes);
  }

  public int CompareTo(object? obj)
  {
    if (ReferenceEquals(null, obj)) return 1;

    return obj is Size other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Size)}");
  }

  public static Size operator +(Size a, Size b) => new(a.Bytes + b.Bytes);
  public static Size operator -(Size a, Size b) => new(a.Bytes - b.Bytes);

  public static bool operator <(Size left, Size right) => left.Bytes < right.Bytes;
  public static bool operator >(Size left, Size right) => left.Bytes > right.Bytes;
  public static bool operator <=(Size left, Size right) => left.Bytes <= right.Bytes;
  public static bool operator >=(Size left, Size right) => left.Bytes >= right.Bytes;

  public static implicit operator int(Size size) => (int) size.Bytes;
  public static implicit operator long(Size size) => size.Bytes;
}

/// <summary>Static extensions for <see cref="Size"/>.</summary>
public static class SizeExtensions
{
  public static Size Bytes(this int value) => new(value);

  public static Size Kilobytes(this int value) => Bytes(value * 1024);
  public static Size Megabytes(this int value) => Kilobytes(value * 1024);
  public static Size Gigabytes(this int value) => Megabytes(value * 1024);
  public static Size Terabytes(this int value) => Gigabytes(value * 1024);

  public static Size Sum(this IEnumerable<Size> sizes)
  {
    var totalBytes = 0L;

    foreach (var size in sizes)
    {
      totalBytes += size.Bytes;
    }

    return new Size(totalBytes);
  }

  public static Size CalculateSize<T>(this Span<T> span)
    where T : unmanaged
  {
    return Bytes(span.Length * Unsafe.SizeOf<T>());
  }

  public static Size CalculateSize<T>(this ReadOnlySpan<T> span)
    where T : unmanaged
  {
    return Bytes(span.Length * Unsafe.SizeOf<T>());
  }
}
