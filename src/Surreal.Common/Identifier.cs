using System;

namespace Surreal
{
  /// <summary>Represents a serializable and efficiently packed unique identifier.</summary>
  public readonly record struct Identifier(Guid Id)
  {
    public static Identifier None       => default;
    public static Identifier Randomized => Guid.NewGuid();

    public static implicit operator Identifier(Guid guid) => new(guid);
  }
}
