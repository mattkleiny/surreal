namespace Surreal;

/// <summary>
/// An optional type for <see cref="T" />.
/// </summary>
public readonly struct Optional<T>
{
  public Optional(T? value, bool hasValue)
  {
    Value = value;
    IsSome = hasValue;
  }

  public T? Value { get; }

  public bool IsSome { get; }
  public bool IsNone => !IsSome;

  public T GetOrDefault(T defaultValue)
  {
    if (IsSome)
    {
      return Value!;
    }

    return defaultValue;
  }

  public static implicit operator Optional<T>(T value)
  {
    return new Optional<T>(value, true);
  }
}
