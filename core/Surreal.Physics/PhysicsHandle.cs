using Surreal.Collections;

namespace Surreal.Physics;

/// <summary>
/// An opaque handle to a resource in the underling <see cref="IPhysicsBackend" /> implementation.
/// </summary>
[ExcludeFromCodeCoverage]
public readonly record struct PhysicsHandle(ulong Id)
{
  public static PhysicsHandle None => default;

  public PhysicsHandle(ArenaIndex index)
    : this((ulong)index)
  {
  }

  public static implicit operator int(PhysicsHandle handle) => (int)handle.Id;
  public static implicit operator uint(PhysicsHandle handle) => (uint)handle.Id;
  public static implicit operator nint(PhysicsHandle handle) => (nint)handle.Id;
  public static implicit operator ulong(PhysicsHandle handle) => handle.Id;
  public static implicit operator ArenaIndex(PhysicsHandle handle) => ArenaIndex.FromUlong(handle);
}
