using System.Collections.Generic;

namespace Surreal.IO {
  public static class SizeExtensions {
    public static Size Bytes(this int value) => new(value);

    public static Size Kilobytes(this int value) => Bytes(value * 1024);
    public static Size Megabytes(this int value) => Kilobytes(value * 1024);
    public static Size Gigabytes(this int value) => Megabytes(value * 1024);
    public static Size Terabytes(this int value) => Gigabytes(value * 1024);

    public static Size Sum(this IEnumerable<Size> sizes) {
      var totalBytes = 0L;

      foreach (var size in sizes) {
        totalBytes += size.Bytes;
      }

      return new Size(totalBytes);
    }
  }
}