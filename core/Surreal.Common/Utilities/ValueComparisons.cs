namespace Surreal.Utilities;

/// <summary>Different types of <see cref="ValueComparison{T}"/>.</summary>
public enum ComparisonType
{
  LessThan,
  LessThanOrEqual,
  EqualTo,
  GreaterThan,
  GreaterThanOrEqual
}

/// <summary>A comparison between two values of <see cref="T"/>.</summary>
public readonly record struct ValueComparison<T>(ComparisonType Type, T Value)
  where T : IComparable<T>
{
  public bool Compare(T other) => Type switch
  {
    ComparisonType.LessThan           => other.CompareTo(Value) < 0,
    ComparisonType.LessThanOrEqual    => other.CompareTo(Value) <= 0,
    ComparisonType.EqualTo            => other.CompareTo(Value) == 0,
    ComparisonType.GreaterThan        => other.CompareTo(Value) > 0,
    ComparisonType.GreaterThanOrEqual => other.CompareTo(Value) >= 0,

    _ => throw new InvalidOperationException($"An unrecognized comparison was requested: {Type}"),
  };
}
