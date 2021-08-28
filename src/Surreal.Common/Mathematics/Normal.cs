using System;

namespace Surreal.Mathematics
{
  public readonly struct Normal : IEquatable<Normal>
  {
    public static Normal Min => default;
    public static Normal Max => new(1f);

    public static readonly FloatRange Range = new(0f, 1f);

    public Normal(float value)
    {
      Value = value.Clamp(Range);
    }

    public float Value { get; }

    public override string ToString() => $"<{Value.ToString()}>";

    public          bool Equals(Normal other) => Value.Equals(other.Value);
    public override bool Equals(object? obj)  => obj is Normal other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Normal left, Normal right) => left.Equals(right);
    public static bool operator !=(Normal left, Normal right) => !left.Equals(right);

    public static implicit operator Normal(float value) => new(value);
    public static implicit operator float(Normal time)  => time.Value;
  }
}
