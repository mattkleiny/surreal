using System.Globalization;

namespace Surreal;

/// <summary>Represents an integer identifier; efficiently allocated and unique across the application domain.</summary>
public readonly record struct Identifier(ulong Id)
{
  private static ulong nextId;

  public static Identifier None       => default;
  public static Identifier Allocate() => new(Interlocked.Increment(ref nextId));

  public override string ToString()
  {
    return Id.ToString(CultureInfo.InvariantCulture);
  }
}
