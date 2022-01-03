using System.Runtime.CompilerServices;

namespace Surreal.Memory;

/// <summary>Represents a measure of bytes, convertible to other representations.</summary>
public readonly struct Size
{
  public static readonly Size Zero = new(0);

  public Size(long bytes) => Bytes = bytes;

  public long Bytes { get; }

  public long Kilobytes => Bytes / 1024;
  public long Megabytes => Kilobytes / 1024;
  public long Gigabytes => Megabytes / 1024;
  public long Terabytes => Gigabytes / 1024;

  public static Size operator +(Size a, Size b) => new(a.Bytes + b.Bytes);
  public static Size operator -(Size a, Size b) => new(a.Bytes - b.Bytes);

  public static implicit operator int(Size size)  => (int) size.Bytes;
  public static implicit operator long(Size size) => size.Bytes;

  public override string ToString()
  {
    if (Terabytes > 0) return $"{Terabytes:F)} terabytes";
    if (Gigabytes > 0) return $"{Gigabytes:F)} gigabytes";
    if (Megabytes > 0) return $"{Megabytes:F)} megabytes";
    if (Kilobytes > 0) return $"{Kilobytes:F)} kilobytes";

    return $"{Bytes} bytes";
  }
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
}
