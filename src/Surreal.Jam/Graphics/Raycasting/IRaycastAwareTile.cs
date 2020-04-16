namespace Surreal.Graphics.Raycasting
{
  public interface IRaycastAwareTile
  {
    bool    IsSolid { get; }
    Color   Color   { get; }
    string? Texture { get; }
  }
}