namespace Surreal.Framework.Modifiers {
  public static class Modifier {
    public static Modifier<T> Additive<T>(T amount)   => new(ModifierType.Additive, amount);
    public static Modifier<T> Cumulative<T>(T amount) => new(ModifierType.Cumulative, amount);
    public static Modifier<T> Multiplier<T>(T amount) => new(ModifierType.Multiplicative, amount);
  }

  public readonly struct Modifier<T> {
    public Modifier(ModifierType type, T amount)
        : this(type, amount, (int) type) {
    }

    public Modifier(ModifierType type, T amount, int order) {
      Type   = type;
      Amount = amount;
      Order  = order;
    }

    public readonly ModifierType Type;
    public readonly int          Order;
    public readonly T            Amount;
  }
}