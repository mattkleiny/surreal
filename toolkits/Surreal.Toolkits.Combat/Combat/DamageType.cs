namespace Surreal.Combat;

/// <summary>Indicates a type of damage to be applied from one object to another.</summary>
public readonly record struct DamageType(string Name)
{
  public static DamageType Standard { get; } = new(nameof(Standard));

  public int Hash { get; } = Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

  public bool Equals(DamageType other)
  {
    if (Hash == other.Hash) return true; // TODO: double check

    return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
  }

  public override int GetHashCode()
  {
    return Hash;
  }
}
