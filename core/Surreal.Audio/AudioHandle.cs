﻿using Surreal.Collections;

namespace Surreal.Audio;

/// <summary>
/// An opaque handle to a resource in the underling <see cref="IAudioBackend" /> implementation.
/// </summary>
[ExcludeFromCodeCoverage]
public readonly record struct AudioHandle(ulong Id) : IDisposable
{
  public static AudioHandle None => default;

  public static AudioHandle FromInt(int index) => new((ulong)index);
  public static AudioHandle FromUInt(uint index) => new(index);
  public static AudioHandle FromNInt(nint index) => new((ulong)index);
  public static AudioHandle FromULong(ulong index) => new(index);
  public static AudioHandle FromArenaIndex(ArenaIndex index) => new(index);
  public static AudioHandle FromPointer(IntPtr pointer) => new((ulong)pointer);
  public static unsafe AudioHandle FromPointer(void* pointer) => new((ulong)pointer);

  /// <summary>
  /// Creates a new handle from the given object by pinning it in memory and taking a pointer to it
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static AudioHandle FromObject(object? value)
  {
    var handle = GCHandle.Alloc(value, GCHandleType.Pinned);
    var pointer = GCHandle.ToIntPtr(handle);

    return FromPointer(pointer);
  }

  public static implicit operator int(AudioHandle handle) => (int)handle.Id;
  public static implicit operator uint(AudioHandle handle) => (uint)handle.Id;
  public static implicit operator nint(AudioHandle handle) => (nint)handle.Id;
  public static implicit operator ulong(AudioHandle handle) => handle.Id;
  public static implicit operator ArenaIndex(AudioHandle handle) => ArenaIndex.FromUlong(handle);
  public static unsafe implicit operator void*(AudioHandle handle) => (void*) handle.Id;

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

    handle.Free();
  }
}
