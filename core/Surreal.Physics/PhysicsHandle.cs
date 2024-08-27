using Surreal.Collections;

namespace Surreal.Physics;

/// <summary>
/// An opaque handle to a resource in the underling <see cref="IPhysicsBackend" /> implementation.
/// </summary>
[ExcludeFromCodeCoverage]
public readonly record struct PhysicsHandle(ulong Id)
{
  public static PhysicsHandle None => default;

  public static PhysicsHandle FromInt(int index) => new((ulong)index);
  public static PhysicsHandle FromUInt(uint index) => new(index);
  public static PhysicsHandle FromNInt(nint index) => new((ulong)index);
  public static PhysicsHandle FromULong(ulong index) => new(index);
  public static PhysicsHandle FromArenaIndex(ArenaIndex index) => new(index);
  public static PhysicsHandle FromPointer(IntPtr pointer) => new((ulong)pointer);
  public static unsafe PhysicsHandle FromPointer(void* pointer) => new((ulong)pointer);

  public static implicit operator int(PhysicsHandle handle) => (int)handle.Id;
  public static implicit operator uint(PhysicsHandle handle) => (uint)handle.Id;
  public static implicit operator nint(PhysicsHandle handle) => (nint)handle.Id;
  public static implicit operator ulong(PhysicsHandle handle) => handle.Id;
  public static implicit operator ArenaIndex(PhysicsHandle handle) => ArenaIndex.FromUlong(handle);
  public static unsafe implicit operator void*(PhysicsHandle handle) => (void*)handle.Id;

  public unsafe void* AsPointer() => (void*)Id;
  public unsafe T* AsPointer<T>() where T : unmanaged => (T*)(void*)Id;
}
