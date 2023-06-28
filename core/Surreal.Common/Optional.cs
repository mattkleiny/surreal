namespace Surreal;

/// <summary>
/// An optional type for <see cref="T" />.
/// </summary>
public readonly record struct Optional<T>(T Value, bool IsSome)
{
  public readonly bool IsNone => !IsSome;

  public readonly T GetOrDefault(T defaultValue)
  {
    if (IsSome)
    {
      return Value;
    }

    return defaultValue;
  }

  public static implicit operator Optional<T>(T value)
  {
    return new Optional<T>(value, true);
  }
}
