using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Surreal.Mathematics.Linear;

namespace Minecraft.Core.Coordinates
{
  [DebuggerDisplay("{Distance} chunk radius from {Origin}")]
  public readonly struct Neighborhood : IEnumerable<WorldPos>
  {
    public Neighborhood(Vector3 origin, int distance)
      : this(new Vector3I((int) origin.X, (int) origin.Y, (int) origin.Z), distance)
    {
    }

    public Neighborhood(Vector3I origin, int distance)
    {
      Check.That(distance > 0, "distance > 0");

      Origin   = origin;
      Distance = distance;
    }

    public readonly Vector3I Origin;
    public readonly int    Distance;

    public IEnumerator<WorldPos> GetEnumerator()
    {
      // TODO: move this to an allocation-free enumerator
      var volume = World.VoxelsPerChunk;

      for (var z = Origin.Z / volume.Depth - Distance / 2; z < Origin.Z / volume.Depth + Distance / 2; z++)
      for (var x = Origin.X / volume.Width - Distance / 2; x < Origin.X / volume.Depth + Distance / 2; x++)
      {
        yield return new WorldPos(x * volume.Width, 1, z * volume.Depth);
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}
