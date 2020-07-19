using Isaac.Core.Mobs;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Brains {
  public sealed class SimpletonBrain : Brain {
    public static readonly SimpletonBrain Instance = new SimpletonBrain();

    public override void Think(DeltaTime time, Monster monster) {
      // no-op
    }
  }
}