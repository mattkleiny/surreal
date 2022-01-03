namespace Surreal.Aspects;

/// <summary>Describes an intersection of components for use in component queries.</summary>
public readonly struct Aspect : IEquatable<Aspect>
{
  public static Aspect Of<T1>()
  {
    return new Aspect(HashCode.Combine(typeof(T1)));
  }

  public static Aspect Of<T1, T2>()
  {
    return new Aspect(HashCode.Combine(typeof(T1), typeof(T2)));
  }

  public static Aspect Of<T1, T2, T3>()
  {
    return new Aspect(HashCode.Combine(typeof(T1), typeof(T2), typeof(T3)));
  }

  public static Aspect Of<T1, T2, T3, T4>()
  {
    return new Aspect(HashCode.Combine(typeof(T1), typeof(T2), typeof(T3), typeof(T4)));
  }

  private readonly int hash;

  private Aspect(int hash) => this.hash = hash;

  public          bool Equals(Aspect other) => hash == other.hash;
  public override bool Equals(object? obj)  => obj is Aspect other && Equals(other);

  public override int GetHashCode() => hash;

  public static bool operator ==(Aspect left, Aspect right) => left.Equals(right);
  public static bool operator !=(Aspect left, Aspect right) => !left.Equals(right);
}
