namespace Surreal.Memory;

/// <summary>
/// Indicates a type is capable of estimating it's own <see cref="Size" />.
/// </summary>
public interface IHasSizeEstimate
{
  Size Size { get; }
}

/// <summary>
/// Represents a measure of bytes, convertible to other representations.
/// </summary>
public readonly record struct Size(long Bytes) : IComparable<Size>, IComparable
{
  public static readonly Size Zero = new(0);

  public static Size FromBytes(int value) => new(value);
  public static Size FromKilobytes(int value) => FromBytes(value * 1024);
  public static Size FromMegabytes(int value) => FromKilobytes(value * 1024);
  public static Size FromGigabytes(int value) => FromMegabytes(value * 1024);
  public static Size FromTerabytes(int value) => FromGigabytes(value * 1024);

  public float Kilobytes => Bytes / 1024f;
  public float Megabytes => Kilobytes / 1024f;
  public float Gigabytes => Megabytes / 1024f;
  public float Terabytes => Gigabytes / 1024f;

  public int CompareTo(object? obj)
  {
    if (ReferenceEquals(null, obj))
    {
      return 1;
    }

    return obj is Size other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Size)}");
  }

  public int CompareTo(Size other)
  {
    return Bytes.CompareTo(other.Bytes);
  }

  public override string ToString()
  {
    if (Terabytes > 1) return $"{Terabytes:F} terabytes";
    if (Gigabytes > 1) return $"{Gigabytes:F} gigabytes";
    if (Megabytes > 1) return $"{Megabytes:F} megabytes";
    if (Kilobytes > 1) return $"{Kilobytes:F} kilobytes";

    return $"{Bytes} bytes";
  }

  public static Size operator +(Size a, Size b) => new(a.Bytes + b.Bytes);
  public static Size operator -(Size a, Size b) => new(a.Bytes - b.Bytes);
  public static bool operator <(Size left, Size right) => left.Bytes < right.Bytes;
  public static bool operator >(Size left, Size right) => left.Bytes > right.Bytes;
  public static bool operator <=(Size left, Size right) => left.Bytes <= right.Bytes;
  public static bool operator >=(Size left, Size right) => left.Bytes >= right.Bytes;

  public static implicit operator Size(int bytes) => new(bytes);
  public static implicit operator int(Size size) => (int)size.Bytes;
  public static implicit operator long(Size size) => size.Bytes;
}

/// <summary>
/// Static extensions for <see cref="Size" />.
/// </summary>
public static class SizeExtensions
{
  public static Size Sum(this IEnumerable<Size> sizes)
  {
    var totalBytes = sizes.Sum(size => size.Bytes);

    return new Size(totalBytes);
  }

  public static Size CalculateSize<T>(this Span<T> span)
  {
    return Size.FromBytes(span.Length * Unsafe.SizeOf<T>());
  }

  public static Size CalculateSize<T>(this ReadOnlySpan<T> span)
  {
    return Size.FromBytes(span.Length * Unsafe.SizeOf<T>());
  }
}
