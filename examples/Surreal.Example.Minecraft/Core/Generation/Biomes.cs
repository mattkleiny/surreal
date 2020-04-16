using Minecraft.Core.Coordinates;

namespace Minecraft.Core.Generation
{
  public enum Biome
  {
    Temperate
  }

  public delegate Biome BiomeSelector(RegionPos position);

  public static class BiomeSelectors
  {
    public static BiomeSelector Always(Biome biome) => position => biome;
  }
}
