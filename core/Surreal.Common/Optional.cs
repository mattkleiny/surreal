namespace Surreal;

/// <summary>
/// Static factory for <see cref="Optional{T}"/> values.
/// </summary>
public static class Optional
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Optional<T> Some<T>(T value) => value;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Optional<T> None<T>() => default;
}

/// <summary>
/// An optional type for <see cref="T" />.
/// </summary>
public readonly record struct Optional<T>(T Value, bool HasValue)
{
  public T GetOrDefault(T defaultValue)
  {
    if (!HasValue)
    {
      return defaultValue;
    }

    return Value;
  }

  public static implicit operator Optional<T>(T value) => new(value, true);
}
