using System.Runtime.CompilerServices;

namespace Surreal.Framework.Modifiers
{
  public static class Modifier
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Modifier<T> Additive<T>(T amount)
      => new Modifier<T>(ModifierType.Additive, amount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Modifier<T> Cumulative<T>(T amount)
      => new Modifier<T>(ModifierType.Cumulative, amount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Modifier<T> Multiplier<T>(T amount)
      => new Modifier<T>(ModifierType.Multiplicative, amount);
  }

  public readonly struct Modifier<T>
  {
    public Modifier(ModifierType type, T amount)
      : this(type, amount, (int) type)
    {
    }

    public Modifier(ModifierType type, T amount, int order)
    {
      Type   = type;
      Amount = amount;
      Order  = order;
    }

    public readonly ModifierType Type;
    public readonly int          Order;
    public readonly T            Amount;
  }
}