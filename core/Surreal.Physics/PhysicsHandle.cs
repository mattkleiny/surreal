using Surreal.Collections;

namespace Surreal.Physics;

/// <summary>
/// An opaque handle to a resource in the underling <see cref="IPhysicsBackend" /> implementation.
/// </summary>
[ExcludeFromCodeCoverage]
public readonly record struct PhysicsHandle(ulong Id) : IDisposable
{
  public static PhysicsHandle None => default;

  public static PhysicsHandle FromInt(int index) => new((ulong)index);
  public static PhysicsHandle FromUInt(uint index) => new(index);
  public static PhysicsHandle FromNInt(nint index) => new((ulong)index);
  public static PhysicsHandle FromULong(ulong index) => new(index);
  public static PhysicsHandle FromArenaIndex(ArenaIndex index) => new(index);
  public static PhysicsHandle FromPointer(IntPtr pointer) => new((ulong)pointer);
  public static unsafe PhysicsHandle FromPointer(void* pointer) => new((ulong)pointer);

  /// <summary>
  /// Creates a new handle from the given object by pinning it in memory and taking a pointer to it
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PhysicsHandle FromObject(object? value)
  {
    var handle = GCHandle.Alloc(value, GCHandleType.Pinned);
    var pointer = GCHandle.ToIntPtr(handle);

    return FromPointer(pointer);
  }

  public static implicit operator int(PhysicsHandle handle) => (int)handle.Id;
  public static implicit operator uint(PhysicsHandle handle) => (uint)handle.Id;
  public static implicit operator nint(PhysicsHandle handle) => (nint)handle.Id;
  public static implicit operator ulong(PhysicsHandle handle) => handle.Id;
  public static implicit operator ArenaIndex(PhysicsHandle handle) => ArenaIndex.FromUlong(handle);
  public static unsafe implicit operator void*(PhysicsHandle handle) => (void*) handle.Id;

  /// <summary>
  /// Converts the handle to an integer.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public unsafe void* AsPointer() => (void*)Id;

  /// <summary>
  /// Converts the handle to a pointer of type <typeparamref name="T" />.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public unsafe T* AsPointer<T>() where T : unmanaged => (T*)(void*)Id;

  /// <summary>
  /// Converts the handle to an object of type <typeparamref name="T" />.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public T AsObject<T>() where T : class
  {
    var handle = GCHandle.FromIntPtr(this);
    var target = handle.Target;

    if (target == null)
    {
      throw new NullReferenceException("The object pointed at by this handle is null.");
    }

    return (T) target;
  }

  /// <summary>
  /// Frees the GC handle of the value pointed at by this handle, assuming it's a pointer.
  /// </summary>
  public void Dispose()
  {
    var handle = GCHandle.FromIntPtr(this);
    if (handle.IsAllocated)
    {
      if (handle.Target is IDisposable disposable)
      {
        disposable.Dispose();
      }

      handle.Free();
    }
  }
}
