using Surreal.Collections;

namespace Surreal.Audio;

/// <summary>
/// An opaque handle to a resource in the underling <see cref="IAudioBackend" /> implementation.
/// </summary>
[ExcludeFromCodeCoverage]
public readonly record struct AudioHandle(ulong Id)
{
  public static AudioHandle None => default;

  public static AudioHandle FromInt(int index) => new((ulong)index);
  public static AudioHandle FromUInt(uint index) => new(index);
  public static AudioHandle FromNInt(nint index) => new((ulong)index);
  public static AudioHandle FromULong(ulong index) => new(index);
  public static AudioHandle FromArenaIndex(ArenaIndex index) => new(index);
  public static AudioHandle FromPointer(IntPtr pointer) => new((ulong)pointer);
  public static unsafe AudioHandle FromPointer(void* pointer) => new((ulong)pointer);

  public static implicit operator int(AudioHandle handle) => (int)handle.Id;
  public static implicit operator uint(AudioHandle handle) => (uint)handle.Id;
  public static implicit operator nint(AudioHandle handle) => (nint)handle.Id;
  public static implicit operator ulong(AudioHandle handle) => handle.Id;
  public static implicit operator ArenaIndex(AudioHandle handle) => ArenaIndex.FromUlong(handle);
  public static unsafe implicit operator void*(AudioHandle handle) => (void*)handle.Id;

  public unsafe void* AsPointer() => (void*)Id;
  public unsafe T* AsPointer<T>() where T : unmanaged => (T*)(void*)Id;
}
