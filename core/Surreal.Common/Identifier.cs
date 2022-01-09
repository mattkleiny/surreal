using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Surreal;

/// <summary>Static facade for <see cref="Identifier"/>s.</summary>
public static class Identifier
{
  /// <summary>Allocates a new identifier, unique statically and only in this application domain.</summary>
  public static Identifier<T> Allocate<T>()
  {
    return new(Interlocked.Increment(ref Identifier<T>.nextId));
  }
}

/// <summary>Represents an integer identifier; efficiently allocated and unique across the application domain.</summary>
[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly record struct Identifier<T>(ulong Id)
{
  internal static ulong nextId = 1;

  public bool IsValid   => Id > 0;
  public bool IsInvalid => !IsValid;

  public override string ToString()
  {
    return Id.ToString(CultureInfo.InvariantCulture);
  }
}
