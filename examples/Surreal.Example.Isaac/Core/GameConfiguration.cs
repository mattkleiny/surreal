using Surreal.Mathematics;

namespace Isaac.Core
{
  public class GameConfiguration
  {
    public virtual Seed Seed        { get; set; } = default;
    public virtual int  FloorWidth  { get; set; } = 16;
    public virtual int  FloorHeight { get; set; } = 16;
  }
}