using Isaac.Core.Mobs;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Brains {
  public abstract class Brain {
    public static Brain Null { get; } = new NullBrain();

    public abstract void Think(DeltaTime time, Monster monster);

    private sealed class NullBrain : Brain {
      public override void Think(DeltaTime time, Monster monster) {
        // no-op
      }
    }
  }
}