namespace Surreal.Framework.Voxels
{
  public interface IVoxelPalette<TVoxel>
  {
    TVoxel Empty { get; }
    int    Count { get; }

    TVoxel this[ushort id] { get; }
    ushort this[TVoxel voxel] { get; }
  }
}
