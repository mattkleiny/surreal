namespace Surreal.IO {
  public readonly struct Size {
    public static readonly Size Zero = new(0);

    public Size(long bytes) => Bytes = bytes;

    public long Bytes { get; }

    public long Kilobytes => Bytes     / 1024;
    public long Megabytes => Kilobytes / 1024;
    public long Gigabytes => Megabytes / 1024;
    public long Terabytes => Gigabytes / 1024;

    public static Size operator +(Size a, Size b) => new(a.Bytes + b.Bytes);
    public static Size operator -(Size a, Size b) => new(a.Bytes - b.Bytes);

    public static implicit operator int(Size size)  => (int) size.Bytes;
    public static implicit operator long(Size size) => size.Bytes;

    public override string ToString() {
      if (Terabytes > 0) return $"{Terabytes:F} terabytes";
      if (Gigabytes > 0) return $"{Gigabytes:F} gigabytes";
      if (Megabytes > 0) return $"{Megabytes:F} megabytes";
      if (Kilobytes > 0) return $"{Kilobytes:F} kilobytes";

      return $"{Bytes} bytes";
    }
  }
}