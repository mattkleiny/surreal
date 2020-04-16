using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Surreal.Framework.Voxels
{
  public sealed class ReflectiveVoxelPalette<TVoxel> : IVoxelPalette<TVoxel>
    where TVoxel : class, IHasId
  {
    private readonly Dictionary<ushort, TVoxel> voxelById;
    private readonly Dictionary<TVoxel, ushort> idByVoxel;

    public ReflectiveVoxelPalette(IReflect container)
    {
      // scan all static members of the given type and add to container
      var results = container.GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(field => typeof(TVoxel).IsAssignableFrom(field.FieldType))
        .Select(field => (TVoxel) field.GetValue(null))
        .ToList();

      voxelById = results.ToDictionary(block => block.Id, block => block);
      idByVoxel = results.ToDictionary(block => block, block => block.Id);
    }

    public TVoxel Empty => this[0];
    public int    Count => voxelById.Count;

    public ushort this[TVoxel voxel] => idByVoxel[voxel];
    public TVoxel this[ushort id] => voxelById[id];
  }
}
