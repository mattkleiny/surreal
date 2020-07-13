namespace Sunsets.Core {
  public interface IPersona {
  }

  public interface ISchedule {
    ScheduleEntry GetEntry(WorldTime time);
  }

  public readonly struct ScheduleEntry {
  }

  public readonly struct Radix {
    public int Value { get; }
    public int Bound { get; }

    public Radix(int value, int bound) {
      Bound = bound;
      Value = value % bound;
    }

    public static implicit operator int(Radix radix) => radix.Value;
  }

  public readonly struct WorldTime {
  }
}