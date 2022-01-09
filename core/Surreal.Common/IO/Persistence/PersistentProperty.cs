namespace Surreal.IO.Persistence;

/// <summary>A strongly-typed id for a persistent object property.</summary>
public readonly record struct PersistentProperty<T>(string Name, T? DefaultValue = default)
{
  public override string ToString()    => Name;
  public override int    GetHashCode() => Name.GetHashCode();

  public bool Equals(PersistentProperty<T> other) => string.Equals(Name, other.Name, StringComparison.Ordinal);
}
