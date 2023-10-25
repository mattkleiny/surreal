namespace Surreal.Memory;

/// <summary>
/// A boxed value type <typeparamref name="T"/> on the managed heap.
/// <para/>
/// This can be used to store a value type <typeparamref name="T"/> on the managed heap,
/// and make it explicit instead of boxing it generically to a <see cref="object"/>.
/// </summary>
[DebuggerDisplay("{ToString(),raw}")]
public sealed class Box<T>
    where T : struct
{
  /// <summary>
  /// Returns a <see cref="Box{T}"/> reference from the input <see cref="object"/> instance.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Box<T> GetFrom(object obj)
  {
    if (obj.GetType() != typeof(T))
    {
      throw new InvalidCastException($"Can't cast the input object to the type Box<{typeof(T)}>");
    }

    return Unsafe.As<Box<T>>(obj);
  }

  /// <summary>
  /// Tries to get a <see cref="Box{T}"/> reference from an input <see cref="object"/> representing a boxed <typeparamref name="T"/> value.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryGetFrom(object obj, [NotNullWhen(true)] out Box<T>? result)
  {
    if (obj.GetType() == typeof(T))
    {
      result = Unsafe.As<Box<T>>(obj);
      return true;
    }

    result = null;
    return false;
  }

  /// <summary>
  /// Returns a <see cref="Box{T}"/> reference from the input <see cref="object"/> instance.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Box<T> UnsafeGetFrom(object obj)
  {
    return Unsafe.As<Box<T>>(obj);
  }

  public override string ToString()
  {
    return this.GetReference().ToString()!;
  }

  public override bool Equals(object? obj)
  {
    return Equals(this, obj);
  }

  public override int GetHashCode()
  {
    return this.GetReference().GetHashCode();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator T(Box<T> box) => (T)(object)box;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Box<T>(T value) => Unsafe.As<Box<T>>(value);
}

/// <summary>
/// Helpers for working with the <see cref="Box{T}"/> type.
/// </summary>
public static class BoxExtensions
{
  /// <summary>
  /// Gets a <typeparamref name="T"/> reference from a <see cref="Box{T}"/> instance.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ref T GetReference<T>(this Box<T> box)
      where T : struct
  {
    return ref Unsafe.Unbox<T>(box);
  }
}
