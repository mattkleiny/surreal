namespace Minecraft.Worlds;

public readonly record struct Volume(int Width, int Height, int Depth)
{
  public int Total => Width * Height * Depth;
}
