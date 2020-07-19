using Isaac.Core.Mobs;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Brains {
  public abstract class Brain {
    public abstract void Think(DeltaTime time, Monster monster);
  }
}