namespace Surreal.Collections.Blackboards;

/// <summary>Identifies a single property in a <see cref="IBlackboard"/>, with a default value.</summary>
public readonly record struct BlackboardProperty<T>(string Key, T? DefaultValue = default)
{
  public override int GetHashCode()
  {
    return string.GetHashCode(Key, StringComparison.OrdinalIgnoreCase);
  }

  public bool Equals(BlackboardProperty<T> other)
  {
    return string.Equals(Key, other.Key, StringComparison.OrdinalIgnoreCase);
  }
}