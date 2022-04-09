namespace Surreal;

/// <summary>An optional type for <see cref="T"/>.</summary>
public readonly struct Optional<T>
{
  private readonly T? value;
  private readonly bool hasValue;

  public Optional(T? value, bool hasValue)
  {
    this.value = value;
    this.hasValue = hasValue;
  }

  public T?   Value  => value;
  public bool IsSome => hasValue;
  public bool IsNone => !hasValue;

  public T GetOrDefault(T defaultValue)
  {
    if (IsSome)
    {
      return value!;
    }

    return defaultValue;
  }

  public static implicit operator Optional<T>(T value) => new(value, true);
}
